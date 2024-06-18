using inercya.EntityLite;
using System.Data;

namespace MassTransit.EntityLiteIntegration
{
    [SqlEntity(BaseTableName = "inbox_states", SchemaName = "mt")]
    public class InboxState
    {
        /// <summary>
        /// Primary key for table, to have ordered clustered index
        /// </summary>
        [SqlField(DbType.Int64, 8, Precision = 19, IsKey = true, SequenceName = "inbox_states_id_seq", ColumnName = "id", BaseColumnName = "id", BaseTableName = "inbox_states")]
        public long Id { get; set; }

        /// <summary>
        /// The MessageId of the incoming message
        /// </summary>
        [SqlField(DbType.Guid, 16, ColumnName = "message_id", BaseColumnName = "message_id", BaseTableName = "inbox_states")]
        public Guid MessageId { get; set; }

        /// <summary>
        /// And MD5 hash of the endpoint name + consumer type
        /// </summary>
        [SqlField(DbType.Guid, 16, ColumnName = "consumer_id", BaseColumnName = "consumer_id", BaseTableName = "inbox_states")]
        public Guid ConsumerId { get; set; }

        /// <summary>
        /// Lock token to ensure row is locked within the transaction
        /// </summary>
        [SqlField(DbType.Guid, 16, ColumnName = "lock_id", BaseColumnName = "lock_id", BaseTableName = "inbox_states")]
        public Guid LockId { get; set; }

        /// <summary>
        /// EntityLite RowVersion
        /// </summary>
        [SqlField(DbType.Int32, 4, Precision = 10, ColumnName = "entity_row_version", BaseColumnName = "entity_row_version", BaseTableName = "inbox_states")]
        public int EntityRowVersion { get; set; }

        /// <summary>
        /// When the message was first received
        /// </summary>
        [SqlField(DbType.DateTime, 8, Precision = 23, Scale = 3, ColumnName = "received", BaseColumnName = "received", BaseTableName = "inbox_states")]
        public DateTime Received { get; set; }

        /// <summary>
        /// How many times the message has been received
        /// </summary>
        [SqlField(DbType.Int32, 4, Precision = 10, ColumnName = "receive_count", BaseColumnName = "receive_count", BaseTableName = "inbox_states")]
        public int ReceiveCount { get; set; }

        /// <summary>
        /// If present, when the message expires (from the message header)
        /// </summary>
        [SqlField(DbType.DateTime, 8, Precision = 23, Scale = 3, AllowNull = true, ColumnName = "expiration_time", BaseColumnName = "expiration_time", BaseTableName = "inbox_states")]
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// When the message was consumed, successfully
        /// </summary>
        [SqlField(DbType.DateTime, 8, Precision = 23, Scale = 3, AllowNull = true, ColumnName = "consumed", BaseColumnName = "consumed", BaseTableName = "inbox_states")]
        public DateTime? Consumed { get; set; }


        /// <summary>
        /// When all messages in the outbox were delivered to the transport
        /// </summary>

        [SqlField(DbType.DateTime, 8, Precision = 23, Scale = 3, AllowNull = true, ColumnName = "delivered", BaseColumnName = "delivered", BaseTableName = "inbox_states")]
        public DateTime? Delivered { get; set; }

        /// <summary>
        /// The last sequence number that was successfully delivered to the transport
        /// </summary>
        [SqlField(DbType.Int64, 8, Precision = 19, AllowNull = true, ColumnName = "last_sequence_number", BaseColumnName = "last_sequence_number", BaseTableName = "inbox_states")]
        public long? LastSequenceNumber { get; set; }
    }


}
