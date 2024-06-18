using inercya.EntityLite;
using System.Data;

namespace MassTransit.EntityLiteIntegration
{
    /// <summary>
    /// Used by the sweeper to track the state of an outbox, to ensure that it is properly locked
    /// across sweeper instances to ensure in-order delivery of messages from the outbox.
    /// </summary>
    [SqlEntity(BaseTableName = "outbox_states", SchemaName = "mt")]
    public partial class OutboxState
    {
        /// <summary>
        /// Assigned when the scope is created for an outbox
        /// </summary>
        [SqlField(DbType.Guid, 16, IsKey = true, ColumnName = "outbox_id", BaseColumnName = "outbox_id", BaseTableName = "outbox_states")]
        public Guid OutboxId { get; set; }

        /// <summary>
        /// Lock token to ensure row is locked within the transaction
        /// </summary>
        [SqlField(DbType.Guid, 16, ColumnName = "lock_id", BaseColumnName = "lock_id", BaseTableName = "outbox_states")]
        public Guid LockId { get; set; }

        /// <summary>
        /// EntityLite RowVersion
        /// </summary>
        [SqlField(DbType.Int32, 4, Precision = 10, ColumnName = "entity_row_version", BaseColumnName = "entity_row_version", BaseTableName = "outbox_states")]
        public int EntityRowVersion { get; set; }

        /// <summary>
        /// The point at which the outbox was created
        /// </summary>
        [SqlField(DbType.DateTime, 8, Precision = 23, Scale = 3, ColumnName = "created", BaseColumnName = "created", BaseTableName = "outbox_states")]
        public DateTime Created { get; set; }

        /// <summary>
        /// When all messages in the outbox were delivered to the transport
        /// </summary>
        [SqlField(DbType.DateTime, 8, Precision = 23, Scale = 3, AllowNull = true, ColumnName = "delivered", BaseColumnName = "delivered", BaseTableName = "outbox_states")]
        public DateTime? Delivered { get; set; }

        /// <summary>
        /// The last sequence number that was successfully delivered to the transport
        /// </summary>
        [SqlField(DbType.Int64, 8, Precision = 19, AllowNull = true, ColumnName = "last_sequence_number", BaseColumnName = "last_sequence_number", BaseTableName = "outbox_states")]
        public long? LastSequenceNumber { get; set; }

    }
}
