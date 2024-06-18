#nullable enable
namespace MassTransit.EntityLiteIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using inercya.EntityLite;
    using Middleware;
    using Middleware.Outbox;


    public class EntityLiteOutboxConsumeContext<TDataaService, TMessage> :
        OutboxConsumeContextProxy<TMessage>
        where TDataaService : DataService
        where TMessage : class
    {
        readonly TDataaService _ds;
        readonly InboxState _inboxState;
        readonly Repository<InboxState> inboxRepo;
        public EntityLiteOutboxConsumeContext(ConsumeContext<TMessage> context, OutboxConsumeOptions options, IServiceProvider provider, TDataaService ds,
            InboxState inboxState)
            : base(context, options, provider)
        {
            _ds = ds;
            _inboxState = inboxState;
            inboxRepo = new Repository<InboxState>(_ds);
        }

        public override Guid? MessageId => _inboxState.MessageId;

        public override bool ContinueProcessing { get; set; } = true;

        public override bool IsMessageConsumed => _inboxState.Consumed.HasValue;
        public override bool IsOutboxDelivered => _inboxState.Delivered.HasValue;
        public override int ReceiveCount => _inboxState.ReceiveCount;
        public override long? LastSequenceNumber => _inboxState.LastSequenceNumber;

        // public Guid TransactionId => _transaction.TransactionId;

        public override async Task SetConsumed()
        {

            _inboxState.Consumed = DateTime.UtcNow;
            await inboxRepo.UpdateAsync(_inboxState, nameof(InboxState.Consumed));
            LogContext.Debug?.Log("DbContextOutboxConsumeContext: Inbox Consumed, MessageId: {MessageId}, ConsumerId: {ConsumerId}, Consumed: {Consumed}", MessageId, _inboxState.ConsumerId, _inboxState.Consumed);
        }

        public override async Task SetDelivered()
        {
            _inboxState.Delivered = DateTime.UtcNow;
            await inboxRepo.UpdateAsync(_inboxState, nameof(InboxState.Delivered));
            LogContext.Debug?.Log("DbContextOutboxConsumeContext: Inbox Delivered, MessageId: {MessageId}, ConsumerId: {ConsumerId}, Consumed: {Consumed}", MessageId, ConsumerId, _inboxState.Consumed);
        }

        public override async Task<List<OutboxMessageContext>> LoadOutboxMessages()
        {
            var lastSequenceNumber = LastSequenceNumber ?? 0;

            var messages = await new QueryLite<OutboxMessage>(Projection.BaseTable, _ds)
                .Where(nameof(OutboxMessage.InboxMessageId), OperatorLite.Equals, MessageId)
                .And(nameof(OutboxMessage.InboxConsumerId), OperatorLite.Equals, ConsumerId)
                .And(nameof(OutboxMessage.SequenceNumber), OperatorLite.Greater, lastSequenceNumber)
                .OrderBy(nameof(OutboxMessage.SequenceNumber))
                .ToListAsync(0, Options.MessageDeliveryLimit)
                .ConfigureAwait(false);

            for (var i = 0; i < messages.Count; i++)
                messages[i].Deserialize(SerializerContext);

            LogContext.Debug?.Log("DbContextOutboxConsumeContext: Outbox loaded {Count} messages, MessageId: {MessageId} ConsumerId: {ConsumerId}", messages.Count, MessageId, ConsumerId);

            return messages.Cast<OutboxMessageContext>().ToList();
        }

        public override async Task NotifyOutboxMessageDelivered(OutboxMessageContext message)
        {
            _inboxState.LastSequenceNumber = message.SequenceNumber;
            await this.inboxRepo.UpdateAsync(_inboxState, nameof(InboxState.LastSequenceNumber));
        }

        public override async Task RemoveOutboxMessages()
        {
            var removed = await new QueryLite<OutboxMessage>(Projection.BaseTable, _ds)
                .Where(nameof(OutboxMessage.InboxMessageId), OperatorLite.Equals, MessageId)
                .And(nameof(OutboxMessage.InboxConsumerId), OperatorLite.Equals, ConsumerId)
                .DeleteAsync().ConfigureAwait(false);


            if (removed > 0)
                LogContext.Debug?.Log("Outbox removed {Count} messages: {MessageId}", removed, MessageId);
        }

        public override async Task AddSend<T>(SendContext<T> context)
            where T : class
        {
            await _ds.AddSend(context, SerializerContext, MessageId, ConsumerId);
            LogContext.Debug?.Log("Outbox message added referencing inbox. MessageId: {MessageId}, ConsumerId: {ConsumerId}", MessageId, ConsumerId);
        }
    }
}
