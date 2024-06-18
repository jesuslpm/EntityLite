﻿#nullable enable
namespace MassTransit
{
    using System;
    using Configuration;
    using DependencyInjection;
    using inercya.EntityLite;
    using MassTransit.EntityLiteIntegration;

    public static class EntityLiteOutboxConfigurationExtensions
    {
        /// <summary>
        /// Configures the EntityLite Outbox on the bus, which can subsequently be used to configure
        /// the transactional outbox on a receive endpoint.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TDataService"></typeparam>
        /// <returns></returns>
        public static void AddEntityLiteOutbox<TDataService>(this IBusRegistrationConfigurator configurator,
            Action<IEntityLiteOutboxConfigurator>? configure = null)
            where TDataService : DataService
        {
            var outboxConfigurator = new EntityLiteOutboxConfigurator<TDataService>(configurator);

            outboxConfigurator.Configure(configure);
        }

        /// <summary>
        /// Configure the EntityLite outbox on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">Configuration service provider</param>
        /// <param name="configure"></param>
        public static void UseEntityLiteOutbox<TDataService>(this IReceiveEndpointConfigurator configurator, IRegistrationContext context,
            Action<IOutboxOptionsConfigurator>? configure = null)
            where TDataService : DataService
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var observer = new OutboxConsumePipeSpecificationObserver<TDataService>(configurator, context);

            configure?.Invoke(observer);

            configurator.ConnectConsumerConfigurationObserver(observer);
            configurator.ConnectSagaConfigurationObserver(observer);
        }



        /// <summary>
        /// Configure the outbox for use with SQL Server
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IEntityLiteOutboxConfigurator UseSqlServer(this IEntityLiteOutboxConfigurator configurator)
        {
            configurator.LockStatementProvider = new SqlServerLockStatementProvider();

            return configurator;
        }


        ///// <summary>
        ///// Configure the outbox for use with Postgres
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <returns></returns>
        //public static IEntityFrameworkOutboxConfigurator UsePostgres(this IEntityFrameworkOutboxConfigurator configurator)
        //{
        //    configurator.LockStatementProvider = new PostgresLockStatementProvider();

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the outbox for use with Postgres
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <param name="enableSchemaCaching">Set to false when using multiple DbContexts</param>
        ///// <returns></returns>
        //public static IEntityFrameworkOutboxConfigurator UsePostgres(this IEntityFrameworkOutboxConfigurator configurator, bool enableSchemaCaching)
        //{
        //    configurator.LockStatementProvider = new PostgresLockStatementProvider(enableSchemaCaching);

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the outbox for use with MySQL
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <returns></returns>
        //public static IEntityFrameworkOutboxConfigurator UseMySql(this IEntityFrameworkOutboxConfigurator configurator)
        //{
        //    configurator.LockStatementProvider = new MySqlLockStatementProvider();

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the outbox for use with MySQL
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <param name="enableSchemaCaching">Set to false when using multiple DbContexts</param>
        ///// <returns></returns>
        //public static IEntityFrameworkOutboxConfigurator UseMySql(this IEntityFrameworkOutboxConfigurator configurator, bool enableSchemaCaching)
        //{
        //    configurator.LockStatementProvider = new MySqlLockStatementProvider(enableSchemaCaching);

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the outbox for use with SQLite
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <returns></returns>
        //public static IEntityFrameworkOutboxConfigurator UseSqlite(this IEntityFrameworkOutboxConfigurator configurator)
        //{
        //    configurator.LockStatementProvider = new SqliteLockStatementProvider();

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the outbox for use with SQLite
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <param name="enableSchemaCaching">Set to false when using multiple DbContexts</param>
        ///// <returns></returns>
        //public static IEntityFrameworkOutboxConfigurator UseSqlite(this IEntityFrameworkOutboxConfigurator configurator, bool enableSchemaCaching)
        //{
        //    configurator.LockStatementProvider = new SqliteLockStatementProvider(enableSchemaCaching);

        //    return configurator;
        //}

        ///// <summary>
        ///// Adds all three entities (<see cref="InboxState"/>, <see cref="OutboxState"/>, and <see cref="OutboxMessage"/>)
        ///// to the DbContext. If this method is used, the <see cref="AddInboxStateEntity"/>, <see cref="AddOutboxStateEntity"/>, and
        ///// <see cref="AddOutboxMessageEntity"/> methods should not be used.
        ///// </summary>
        ///// <param name="modelBuilder"></param>
        ///// <param name="callback">Optional, to customize all three entity model builders</param>
        //public static void AddTransactionalOutboxEntities(this ModelBuilder modelBuilder, Action<EntityTypeBuilder>? callback = null)
        //{
        //    modelBuilder.AddInboxStateEntity(callback);
        //    modelBuilder.AddOutboxStateEntity(callback);
        //    modelBuilder.AddOutboxMessageEntity(callback);
        //}

        ///// <summary>
        ///// Adds the <see cref="InboxState"/> entity to the DbContext. If used, the <see cref="AddTransactionalOutboxEntities"/> method should not be used.
        ///// </summary>
        ///// <param name="modelBuilder"></param>
        ///// <param name="callback">Optional, to customize the entity model builder</param>
        //public static void AddInboxStateEntity(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<InboxState>>? callback = null)
        //{
        //    EntityTypeBuilder<InboxState> inbox = modelBuilder.Entity<InboxState>();

        //    inbox.ConfigureInboxStateEntity();

        //    callback?.Invoke(inbox);
        //}

        ///// <summary>
        ///// Configures the <see cref="InboxState"/> entity using an already created <see cref="ModelBuilder"/>.
        ///// </summary>
        ///// <param name="inbox">The model builder</param>
        //public static void ConfigureInboxStateEntity(this EntityTypeBuilder<InboxState> inbox)
        //{
        //    inbox.Property(p => p.Id);
        //    inbox.HasKey(p => p.Id);

        //    inbox.Property(p => p.MessageId);
        //    inbox.Property(p => p.ConsumerId);

        //    inbox.HasAlternateKey(p => new
        //    {
        //        p.MessageId,
        //        p.ConsumerId
        //    });

        //    inbox.Property(p => p.LockId);

        //    inbox.Property(p => p.RowVersion).IsRowVersion();

        //    inbox.Property(p => p.Received);
        //    inbox.Property(p => p.ReceiveCount);
        //    inbox.Property(p => p.ExpirationTime);
        //    inbox.Property(p => p.Consumed);

        //    inbox.Property(p => p.Delivered);
        //    inbox.HasIndex(p => p.Delivered);

        //    inbox.Property(p => p.LastSequenceNumber);
        //}

        ///// <summary>
        ///// Adds the <see cref="OutboxState"/> entity to the DbContext. If used, the <see cref="AddTransactionalOutboxEntities"/> method should not be used.
        ///// </summary>
        ///// <param name="modelBuilder"></param>
        ///// <param name="callback">Optional, to customize the entity model builder</param>
        //public static void AddOutboxStateEntity(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<OutboxState>>? callback = null)
        //{
        //    EntityTypeBuilder<OutboxState> outbox = modelBuilder.Entity<OutboxState>();

        //    outbox.ConfigureOutboxStateEntity();

        //    callback?.Invoke(outbox);
        //}

        ///// <summary>
        ///// Configures the <see cref="OutboxState"/> entity using an already created <see cref="ModelBuilder"/>.
        ///// </summary>
        ///// <param name="outbox">The model builder</param>
        //public static void ConfigureOutboxStateEntity(this EntityTypeBuilder<OutboxState> outbox)
        //{
        //    outbox.Property(p => p.OutboxId);
        //    outbox.HasKey(p => p.OutboxId);

        //    outbox.Property(p => p.LockId);

        //    outbox.Property(p => p.RowVersion).IsRowVersion();

        //    outbox.Property(p => p.Created);
        //    outbox.HasIndex(p => p.Created);

        //    outbox.Property(p => p.Delivered);
        //    outbox.Property(p => p.LastSequenceNumber);
        //}

        ///// <summary>
        ///// Adds the <see cref="OutboxMessage"/> entity to the DbContext. If used, the <see cref="AddTransactionalOutboxEntities"/> method should not be used.
        ///// </summary>
        ///// <param name="modelBuilder"></param>
        ///// <param name="callback">Optional, to customize the entity model builder</param>
        //public static void AddOutboxMessageEntity(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<OutboxMessage>>? callback = null)
        //{
        //    EntityTypeBuilder<OutboxMessage> outbox = modelBuilder.Entity<OutboxMessage>();

        //    outbox.ConfigureOutboxMessageEntity();

        //    callback?.Invoke(outbox);
        //}

        ///// <summary>
        ///// Configures the <see cref="OutboxMessage"/> entity using an already created <see cref="ModelBuilder"/>.
        ///// </summary>
        ///// <param name="outbox">The model builder</param>
        //public static void ConfigureOutboxMessageEntity(this EntityTypeBuilder<OutboxMessage> outbox)
        //{
        //    outbox.Property(p => p.SequenceNumber);
        //    outbox.HasKey(p => p.SequenceNumber);

        //    outbox.Property(p => p.MessageId);

        //    outbox.Property(p => p.ConversationId);
        //    outbox.Property(p => p.CorrelationId);
        //    outbox.Property(p => p.InitiatorId);
        //    outbox.Property(p => p.RequestId);

        //    outbox.Property(p => p.SourceAddress).HasMaxLength(256);
        //    outbox.Property(p => p.DestinationAddress).HasMaxLength(256);
        //    outbox.Property(p => p.ResponseAddress).HasMaxLength(256);
        //    outbox.Property(p => p.FaultAddress).HasMaxLength(256);

        //    outbox.Property(p => p.ExpirationTime);
        //    outbox.HasIndex(p => p.ExpirationTime);

        //    outbox.Property(p => p.EnqueueTime);
        //    outbox.HasIndex(p => p.EnqueueTime);

        //    outbox.Property(p => p.SentTime);

        //    outbox.Property(p => p.InboxMessageId);
        //    outbox.Property(p => p.InboxConsumerId);
        //    outbox.HasIndex(p => new
        //    {
        //        p.InboxMessageId,
        //        p.InboxConsumerId,
        //        p.SequenceNumber
        //    }).IsUnique();

        //    outbox.Property(p => p.OutboxId);
        //    outbox.HasIndex(p => new
        //    {
        //        p.OutboxId,
        //        p.SequenceNumber,
        //    }).IsUnique();

        //    outbox.Property(p => p.Headers);

        //    outbox.Property(p => p.Properties);

        //    outbox.Property(p => p.ContentType)
        //        .HasMaxLength(256);
        //    outbox.Property(p => p.MessageType);

        //    outbox.Property(p => p.Body);
        //}
    }
}
