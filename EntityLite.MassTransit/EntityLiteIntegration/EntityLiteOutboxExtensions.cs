﻿#nullable enable
namespace MassTransit.EntityLiteIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using inercya.EntityLite;

    public static class EntityLiteOutboxExtensions
    {
        public static async Task AddSend<T>(this DataService ds, SendContext<T> context, IObjectDeserializer deserializer,
            Guid? inboxMessageId = null, Guid? inboxConsumerId = null, Guid? outboxId = null)
            where T : class
        {
            if (context.MessageId.HasValue == false)
                throw new MessageException(typeof(T), "The SendContext MessageId must be present");

            var body = context.Serializer.GetMessageBody(context);

            var now = DateTime.UtcNow;

            var outboxMessage = new OutboxMessage
            {
                MessageId = context.MessageId.Value,
                ConversationId = context.ConversationId,
                CorrelationId = context.CorrelationId,
                InitiatorId = context.InitiatorId,
                RequestId = context.RequestId,
                SourceAddress = context.SourceAddress,
                DestinationAddress = context.DestinationAddress,
                ResponseAddress = context.ResponseAddress,
                FaultAddress = context.FaultAddress,
                SentTime = context.SentTime ?? now,
                ContentType = context.ContentType?.ToString() ?? context.Serialization.DefaultContentType.ToString(),
                MessageType = string.Join(";", context.SupportedMessageTypes),
                Body = body.GetString(),
                InboxMessageId = inboxMessageId,
                InboxConsumerId = inboxConsumerId,
                OutboxId = outboxId
            };

            if (context.TimeToLive.HasValue)
                outboxMessage.ExpirationTime = now + context.TimeToLive;

            if (context.Delay.HasValue)
                outboxMessage.EnqueueTime = now + context.Delay;

            outboxMessage.Headers = deserializer.SerializeDictionary(context.Headers.GetAll());

            if (context is TransportSendContext<T> transportSendContext)
            {
                var properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                transportSendContext.WritePropertiesTo(properties);
                outboxMessage.Properties = deserializer.SerializeDictionary(properties);
            }

            var outboxMessageRepo = new Repository<OutboxMessage>(ds);
            await outboxMessageRepo.InsertAsync(outboxMessage);
        }
    }
}
