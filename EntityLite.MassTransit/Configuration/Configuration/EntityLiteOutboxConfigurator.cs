#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Data;
    using inercya.EntityLite;
    using MassTransit.EntityLiteIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Middleware;


    public class EntityLiteOutboxConfigurator<TDataService> :
        IEntityLiteOutboxConfigurator
        where TDataService : DataService
    {
        readonly IBusRegistrationConfigurator _configurator;
        IsolationLevel _isolationLevel;
        ILockStatementProvider _lockStatementProvider;

        public EntityLiteOutboxConfigurator(IBusRegistrationConfigurator configurator)
        {
            _configurator = configurator;

            _lockStatementProvider = new SqlServerLockStatementProvider();
            _isolationLevel = IsolationLevel.RepeatableRead;
        }

        public TimeSpan DuplicateDetectionWindow { get; set; } = TimeSpan.FromMinutes(30);

        public IsolationLevel IsolationLevel
        {
            set => _isolationLevel = value;
        }

        public ILockStatementProvider LockStatementProvider
        {
            set => _lockStatementProvider = value ?? throw new ConfigurationException("LockStatementProvider must not be null");
        }

        public TimeSpan QueryDelay { get; set; } = TimeSpan.FromSeconds(10);

        public TimeSpan InboxQueryDelay { get; set; } = TimeSpan.FromSeconds(30);

        public int QueryMessageLimit { get; set; } = 100;

        public TimeSpan QueryTimeout { get; set; } = TimeSpan.FromSeconds(30);

        public void DisableInboxCleanupService()
        {
            _configurator.RemoveHostedService<InboxCleanupService<TDataService>>();
        }

        public virtual void UseBusOutbox(Action<IEntityLiteBusOutboxConfigurator>? configure = null)
        {
            var busOutboxConfigurator = new EntityLiteBusOutboxConfigurator<TDataService>(_configurator, this);

            busOutboxConfigurator.Configure(configure);
        }

        public virtual void Configure(Action<IEntityLiteOutboxConfigurator>? configure)
        {
            _configurator.TryAddScoped<IOutboxContextFactory<TDataService>, EntityLiteOutboxContextFactory<TDataService>>();
            _configurator.AddOptions<EntityLiteOutboxOptions>().Configure(options =>
            {
                options.IsolationLevel = _isolationLevel;
                options.LockStatementProvider = _lockStatementProvider;
            });

            _configurator.AddHostedService<InboxCleanupService<TDataService>>();
            _configurator.AddOptions<InboxCleanupServiceOptions>().Configure(options =>
            {
                options.DuplicateDetectionWindow = DuplicateDetectionWindow;
                options.QueryMessageLimit = QueryMessageLimit;
                options.QueryDelay = InboxQueryDelay;
                options.QueryTimeout = QueryTimeout;
            });

            configure?.Invoke(this);
        }
    }
}
