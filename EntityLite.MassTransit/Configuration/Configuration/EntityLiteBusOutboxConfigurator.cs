#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection;
    using EntityLiteIntegration;
    using inercya.EntityLite;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware.Outbox;


    public class EntityLiteBusOutboxConfigurator<TDataService> :
        IEntityLiteBusOutboxConfigurator
        where TDataService : DataService
    {
        readonly IBusRegistrationConfigurator _configurator;
        readonly EntityLiteOutboxConfigurator<TDataService> _outboxConfigurator;

        public EntityLiteBusOutboxConfigurator(IBusRegistrationConfigurator configurator, EntityLiteOutboxConfigurator<TDataService> outboxConfigurator)
        {
            _outboxConfigurator = outboxConfigurator;
            _configurator = configurator;
        }

        /// <summary>
        /// The number of message to deliver at a time from the outbox
        /// </summary>
        public int MessageDeliveryLimit { get; set; } = 100;

        /// <summary>
        /// Transport Send timeout when delivering messages to the transport
        /// </summary>
        public TimeSpan MessageDeliveryTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public void DisableDeliveryService()
        {
            _configurator.RemoveHostedService<BusOutboxDeliveryService<TDataService>>();
        }

        public virtual void Configure(Action<IEntityLiteBusOutboxConfigurator>? configure)
        {
            _configurator.AddHostedService<BusOutboxDeliveryService<TDataService>>();
            _configurator.ReplaceScoped<IScopedBusContextProvider<IBus>, EntityLiteScopedBusContextProvider<IBus, TDataService>>();
            _configurator.AddSingleton<IBusOutboxNotification, BusOutboxNotification>();

            _configurator.AddOptions<OutboxDeliveryServiceOptions>()
                .Configure(options =>
                {
                    options.QueryDelay = _outboxConfigurator.QueryDelay;
                    options.QueryMessageLimit = _outboxConfigurator.QueryMessageLimit;
                    options.QueryTimeout = _outboxConfigurator.QueryTimeout;
                    options.MessageDeliveryLimit = MessageDeliveryLimit;
                    options.MessageDeliveryTimeout = MessageDeliveryTimeout;
                });

            configure?.Invoke(this);
        }
    }
}
