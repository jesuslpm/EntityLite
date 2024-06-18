namespace MassTransit.EntityLiteIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using inercya.EntityLite;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Middleware;
    using Middleware.Outbox;
    using Serialization;
    using Util;


    public class BusOutboxDeliveryService<TDataService> :
        BackgroundService
        where TDataService : DataService
    {
        readonly IBusControl _busControl;
        readonly IsolationLevel _isolationLevel;
        readonly ILockStatementProvider _lockStatementProvider;
        readonly ILogger _logger;
        readonly IBusOutboxNotification _notification;
        readonly OutboxDeliveryServiceOptions _options;
        readonly IServiceProvider _provider;
        readonly int _queryTimeout;


        public BusOutboxDeliveryService(IBusControl busControl, IOptions<OutboxDeliveryServiceOptions> options,
            IOptions<EntityLiteOutboxOptions> outboxOptions, IBusOutboxNotification notification,
            ILogger<BusOutboxDeliveryService<TDataService>> logger, IServiceProvider provider)
        {
            _busControl = busControl;
            _notification = notification;
            _provider = provider;
            _logger = logger;

            _options = options.Value;
            _queryTimeout = (int) _options.QueryTimeout.TotalMilliseconds;

            _lockStatementProvider = outboxOptions.Value.LockStatementProvider;
            _isolationLevel = outboxOptions.Value.IsolationLevel;

        }

        private async Task<List<OutboxMessage>> GetOutboxMessages(TDataService ds, Guid outboxId, long lastSequenceNumber, int limit)
        {
            var messages = await new QueryLite<OutboxMessage>(Projection.BaseTable, ds)
                .Where(nameof(OutboxMessage.OutboxId), OperatorLite.Equals, outboxId)
                .And(nameof(OutboxMessage.SequenceNumber), OperatorLite.Greater, lastSequenceNumber)
                .OrderBy(nameof(OutboxMessage.SequenceNumber))
                .ToListAsync(0, limit - 1);
            return messages;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var algorithm = new RequestRateAlgorithm(new RequestRateAlgorithmOptions
            {
                PrefetchCount = _options.QueryMessageLimit,
                RequestResultLimit = 10,
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _busControl.WaitForHealthStatus(BusHealthStatus.Healthy, stoppingToken).ConfigureAwait(false);

                    var count = await algorithm.Run(DeliverOutbox, stoppingToken).ConfigureAwait(false);
                    if (count > 0)
                        continue;

                    await _notification.WaitForDelivery(stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (DBConcurrencyException)
                {
                }
                catch (InvalidOperationException exception) when (exception.InnerException != null
                                                                  && exception.InnerException.Message.Contains("Concurrency conflict detected"))
                {
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "ProcessMessageBatch faulted");
                }
            }
        }

        async Task<int> DeliverOutbox(int resultLimit, CancellationToken cancellationToken)
        {
            using (var scope = _provider.CreateScope())
            using (var ds = scope.ServiceProvider.GetRequiredService<TDataService>())
            {
                async Task<int> Execute()
                {
                    var lockId = NewId.NextGuid();
                    using var timeoutToken = new CancellationTokenSource(_options.QueryTimeout);
                    ds.BeginTransaction(_isolationLevel);
                    try
                    {
                        var outboxState = await _lockStatementProvider.GetOutboxQuery(ds)
                            .WithTimeout(_queryTimeout)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

                        if (outboxState == null)
                            return -1;

                        outboxState.LockId = lockId;
                        var repo = new Repository<OutboxState>(ds);
                        await repo.UpdateAsync(outboxState, nameof(OutboxState.LockId)).ConfigureAwait(false);
                        int continueProcessing;

                        if (outboxState.Delivered.HasValue)
                        {
                            await RemoveOutbox(ds, outboxState, cancellationToken).ConfigureAwait(false);

                            continueProcessing = 0;
                        }
                        else
                            continueProcessing = await DeliverOutboxMessages(ds, outboxState, cancellationToken).ConfigureAwait(false);

                        ds.Commit();

                        return continueProcessing;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (InvalidOperationException exception) when (exception.InnerException != null
                                                                      && exception.InnerException.Message.Contains("Concurrency conflict detected"))
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        RollbackTransaction(ds);
                        throw;
                    }
                }


                var messageCount = 0;
                while (messageCount < resultLimit)
                {
                    var executeResult = await Execute().ConfigureAwait(false);
                    if (executeResult < 0)
                        break;

                    if (executeResult > 0)
                        messageCount += executeResult;
                }

                return messageCount;
            }
        }

        static async Task RemoveOutbox(TDataService ds, OutboxState outboxState, CancellationToken cancellationToken)
        {
            var removedCount = await new QueryLite<OutboxMessage>(Projection.BaseTable, ds)
                .Where(nameof(OutboxMessage.OutboxId), OperatorLite.Equals, outboxState.OutboxId)
                .DeleteAsync();

            var repo = new Repository<OutboxState>(ds);
            await repo.DeleteAsync(outboxState).ConfigureAwait(false);

            if (removedCount > 0)
                LogContext.Debug?.Log("Outbox removed {Count} messages: {OutboxId}", removedCount, outboxState);
        }

        async Task<int> DeliverOutboxMessages(TDataService ds, OutboxState outboxState, CancellationToken cancellationToken)
        {
            var messageLimit = _options.MessageDeliveryLimit;

            var hasLastSequenceNumber = outboxState.LastSequenceNumber.HasValue;

            var lastSequenceNumber = outboxState.LastSequenceNumber ?? 0;

            var messages = await GetOutboxMessages(ds, outboxState.OutboxId, lastSequenceNumber, messageLimit).ConfigureAwait(false);

            var sentSequenceNumber = 0L;

            var messageCount = 0;
            var messageIndex = 0;

            var messageRepo = new Repository<OutboxMessage>(ds);
            var outboxRepo = new Repository<OutboxState>(ds);
            for (; messageIndex < messages.Count && messageCount < messageLimit; messageIndex++)
            {
                var message = messages[messageIndex];

                message.Deserialize(SystemTextJsonMessageSerializer.Instance);

                if (message.DestinationAddress == null)
                {
                    LogContext.Warning?.Log("Outbox message DestinationAddress not present: {SequenceNumber} {MessageId}", message.SequenceNumber,
                        message.MessageId);
                }
                else
                {
                    try
                    {
                        using var sendToken = new CancellationTokenSource(_options.MessageDeliveryTimeout);
                        using var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, sendToken.Token);

                        var pipe = new OutboxMessageSendPipe(message, message.DestinationAddress);

                        var endpoint = await _busControl.GetSendEndpoint(message.DestinationAddress).ConfigureAwait(false);

                        StartedActivity? activity = LogContext.Current?.StartOutboxDeliverActivity(message);
                        StartedInstrument? instrument = LogContext.Current?.StartOutboxDeliveryInstrument(message);

                        try
                        {
                            await endpoint.Send(new SerializedMessageBody(), pipe, token.Token).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            activity?.AddExceptionEvent(ex);
                            instrument?.AddException(ex);
                            throw;
                        }
                        finally
                        {
                            activity?.Stop();
                            instrument?.Stop();
                        }

                        sentSequenceNumber = message.SequenceNumber;

                        LogContext.Debug?.Log("Outbox Sent: {OutboxId} {SequenceNumber} {MessageId}", message.OutboxId, sentSequenceNumber, message.MessageId);
                    }
                    catch (Exception ex)
                    {
                        LogContext.Warning?.Log(ex, "Outbox Send Fault: {OutboxId} {SequenceNumber} {MessageId}", message.OutboxId, message.SequenceNumber,
                            message.MessageId);

                        break;
                    }

                    messageRepo.Delete(message);
                    messageCount++;
                }
            }

            if (sentSequenceNumber > 0)
            {
                outboxState.LastSequenceNumber = sentSequenceNumber;
                await outboxRepo.UpdateAsync(outboxState, nameof(OutboxState.LastSequenceNumber)).ConfigureAwait(false);
            }

            if (messageIndex == messages.Count && messages.Count < messageLimit)
            {
                outboxState.Delivered = DateTime.UtcNow;

                if (hasLastSequenceNumber == false)
                {
                    await outboxRepo.DeleteAsync(outboxState).ConfigureAwait(false);
                    await new QueryLite<OutboxMessage>(Projection.BaseTable, ds)
                        .Where(nameof(OutboxMessage.SequenceNumber), OperatorLite.In, messages.Select(x => x.SequenceNumber))
                        .DeleteAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    await outboxRepo.UpdateAsync(outboxState, nameof(OutboxState.Delivered)).ConfigureAwait(false);
                }
                LogContext.Debug?.Log("Outbox Delivered: {OutboxId} {Delivered}", outboxState.OutboxId, outboxState.Delivered);
            }

            return messageCount;
        }

        static void RollbackTransaction(DataService ds)
        {
            try
            {
                if (ds.IsActiveTransaction) ds.Rollback();
            }
            catch (Exception innerException)
            {
                LogContext.Warning?.Log(innerException, "Transaction rollback failed");
            }
        }
    }
}
