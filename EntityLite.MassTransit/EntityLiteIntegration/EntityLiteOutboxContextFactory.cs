namespace MassTransit.EntityLiteIntegration
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using inercya.EntityLite;
    using Microsoft.Extensions.Options;
    using Middleware;

    /// <summary>
    /// This class is used whe
    /// </summary>
    /// <typeparam name="TDataService"></typeparam>
    public class EntityLiteOutboxContextFactory<TDataService> :
        IOutboxContextFactory<TDataService>
        where TDataService : DataService
    {
        readonly TDataService _ds;
        readonly IsolationLevel _isolationLevel;
        readonly ILockStatementProvider _lockStatementProvider;
        readonly IServiceProvider _provider;

        public EntityLiteOutboxContextFactory(TDataService ds, IServiceProvider provider, IOptions<EntityLiteOutboxOptions> options)
        {
            _ds = ds;
            _provider = provider;
            _lockStatementProvider = options.Value.LockStatementProvider;
            _isolationLevel = options.Value.IsolationLevel;
        }

        public async Task Send<T>(ConsumeContext<T> context, OutboxConsumeOptions options, IPipe<OutboxConsumeContext<T>> next)
            where T : class
        {
            var messageId = context.GetOriginalMessageId() ?? throw new MessageException(typeof(T), "MessageId required to use the outbox");

            async Task<bool> Execute()
            {
                var lockId = NewId.NextGuid();

                _ds.BeginTransaction(_isolationLevel);

                try
                {

                    var inboxState = await _lockStatementProvider.GetInboxQuery(_ds)
                        .Where(nameof(InboxState.MessageId), OperatorLite.Equals, messageId)
                        .And(nameof(InboxState.ConsumerId), OperatorLite.Equals, options.ConsumerId)
                        .FirstOrDefaultAsync().ConfigureAwait(false);

                    bool continueProcessing;

                    var inboxRepo = new Repository<InboxState>(_ds);

                    if (inboxState == null)
                    {
                        inboxState = new InboxState
                        {
                            MessageId = messageId,
                            ConsumerId = options.ConsumerId,
                            Received = DateTime.UtcNow,
                            LockId = lockId,
                            ReceiveCount = 0
                        };
                        await inboxRepo.InsertAsync(inboxState).ConfigureAwait(false);
                        continueProcessing = true;
                    }
                    else
                    {
                        inboxState.LockId = lockId;
                        inboxState.ReceiveCount++;
                        await inboxRepo.UpdateAsync(inboxState, nameof(InboxState.LockId), nameof(InboxState.ReceiveCount));
                        var outboxContext = new EntityLiteOutboxConsumeContext<TDataService, T>(context, options, _provider, _ds, inboxState);
                        await next.Send(outboxContext).ConfigureAwait(false);
                        continueProcessing = outboxContext.ContinueProcessing;
                    }

                    _ds.Commit();

                    return continueProcessing;
                }
                catch (Exception)
                {
                    try
                    {
                        if (_ds.IsActiveTransaction) _ds.Rollback();
                    }
                    catch (Exception innerException)
                    {
                        LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                    }
                    throw;
                }
            }

            var continueProcessing = true;
            while (continueProcessing)
            {
                continueProcessing = await Execute().ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("outboxContextFactory");
            scope.Add("provider", "entityLite");
        }
    }
}
