
namespace MassTransit.EntityLiteIntegration
{

    using System;
    using inercya.EntityLite;
    using System.Data;
    using System.Collections.Generic;
    using Metadata;
    using Middleware;
    using Serialization;

    [SqlEntity(BaseTableName = "outbox_messages", SchemaName = "mt")]
    public partial class OutboxMessage: OutboxMessageContext
    {
        Headers? _headers;
        IReadOnlyDictionary<string, object>? _properties;

        [SqlField(DbType.Int64, 8, Precision = 19, IsKey = true, SequenceName = "outbox_messages_sequence_number_seq", ColumnName = "sequence_number", BaseColumnName = "sequence_number", BaseTableName = "outbox_messages")]
        public long SequenceNumber { get; set; }

        [SqlField(DbType.Guid, 16, ColumnName = "message_id", BaseColumnName = "message_id", BaseTableName = "outbox_messages")]
        public Guid MessageId { get; set; }

        [SqlField(DbType.Guid, 16, AllowNull = true, ColumnName = "conversation_id", BaseColumnName = "conversation_id", BaseTableName = "outbox_messages")]
        public Guid? ConversationId { get; set; }

        [SqlField(DbType.Guid, 16, AllowNull = true, ColumnName = "correlation_id", BaseColumnName = "correlation_id", BaseTableName = "outbox_messages")]
        public Guid? CorrelationId { get; set; }

        [SqlField(DbType.Guid, 16, AllowNull = true, ColumnName = "initiator_id", BaseColumnName = "initiator_id", BaseTableName = "outbox_messages")]
        public Guid? InitiatorId { get; set; }

        [SqlField(DbType.Guid, 16, AllowNull = true, ColumnName = "request_id", BaseColumnName = "request_id", BaseTableName = "outbox_messages")]
        public Guid? RequestId { get; set; }

        [SqlField(DbType.String, 256, ColumnName = "source_address", BaseColumnName = "source_address", BaseTableName = "outbox_messages")]
        public Uri? SourceAddress { get; set; }

        [SqlField(DbType.String, 256, ColumnName = "destination_address", BaseColumnName = "destination_address", BaseTableName = "outbox_messages")]
        public Uri? DestinationAddress { get; set;}

        [SqlField(DbType.String, 256, ColumnName = "response_address", BaseColumnName = "response_address", BaseTableName = "outbox_messages")]
        public Uri? ResponseAddress { get; set; }

        [SqlField(DbType.String, 256, ColumnName = "fault_address", BaseColumnName = "fault_address", BaseTableName = "outbox_messages")]
        public Uri? FaultAddress { get; set; }

        [SqlField(DbType.DateTime, 8, Precision = 23, Scale = 3, AllowNull = true, ColumnName = "expiration_time", BaseColumnName = "expiration_time", BaseTableName = "outbox_messages")]
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// When the message should be visible / ready to be delivered
        /// </summary>
        [SqlField(DbType.DateTime, 8, Precision = 23, Scale = 3, AllowNull = true, ColumnName = "enqueue_time", BaseColumnName = "enqueue_time", BaseTableName = "outbox_messages")]
        public DateTime? EnqueueTime { get; set; }

        [SqlField(DbType.DateTime, 8, Precision = 23, Scale = 3, ColumnName = "sent_time", BaseColumnName = "sent_time", BaseTableName = "outbox_messages")]
        public DateTime SentTime { get; set; }

        [SqlField(DbType.Guid, 16, AllowNull = true, ColumnName = "inbox_message_id", BaseColumnName = "inbox_message_id", BaseTableName = "outbox_messages")]
        public Guid? InboxMessageId { get; set; }

        [SqlField(DbType.Guid, 16, AllowNull = true, ColumnName = "inbox_consumer_id", BaseColumnName = "inbox_consumer_id", BaseTableName = "outbox_messages")]
        public Guid? InboxConsumerId { get; set; }

        [SqlField(DbType.Guid, 16, AllowNull = true, ColumnName = "outbox_id", BaseColumnName = "outbox_id", BaseTableName = "outbox_messages")]
        public Guid? OutboxId { get; set; }

        [SqlField(DbType.String, 2147483647, ColumnName = "headers", BaseColumnName = "headers", BaseTableName = "outbox_messages")]
        public string? Headers { get; set; }

        [SqlField(DbType.String, 2147483647, ColumnName = "properties", BaseColumnName = "properties", BaseTableName = "outbox_messages")]
        public string? Properties { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [SqlField(DbType.String, 256, ColumnName = "content_type", BaseColumnName = "content_type", BaseTableName = "outbox_messages")]
        public string ContentType { get; set; }

        [SqlField(DbType.String, 4000, ColumnName = "message_type", BaseColumnName = "message_type", BaseTableName = "outbox_messages")]
        public string MessageType { get; set; }

        [SqlField(DbType.String, 2147483647, ColumnName = "body", BaseColumnName = "body", BaseTableName = "outbox_messages")]
        public string Body { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        Guid? MessageContext.MessageId => MessageId;
        DateTime? MessageContext.SentTime => SentTime;
        Headers MessageContext.Headers => _headers ?? EmptyHeaders.Instance;
        HostInfo MessageContext.Host => HostMetadataCache.Host;

        IReadOnlyDictionary<string, object> OutboxMessageContext.Properties => _properties!;

        public void Deserialize(IObjectDeserializer deserializer)
        {
            _headers = DeserializerHeaders(deserializer);
            _properties = DeserializerProperties(deserializer);
        }

        Headers DeserializerHeaders(IObjectDeserializer deserializer)
        {
            Dictionary<string, object?>? headers = deserializer.DeserializeDictionary<object?>(Headers);
            if (headers != null)
                return new DictionarySendHeaders(headers);

            return EmptyHeaders.Instance;
        }

        IReadOnlyDictionary<string, object> DeserializerProperties(IObjectDeserializer deserializer)
        {
            Dictionary<string, object>? properties = deserializer.DeserializeDictionary<object>(Properties);

            return properties ?? OutboxMessageStaticData.Empty;
        }
    }

    static class OutboxMessageStaticData
    {
        public static IReadOnlyDictionary<string, object> Empty { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }
}
