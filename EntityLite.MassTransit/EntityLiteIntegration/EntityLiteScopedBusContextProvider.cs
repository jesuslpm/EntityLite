namespace MassTransit.EntityLiteIntegration
{
    using System;
    using DependencyInjection;
    using inercya.EntityLite;
    using Middleware.Outbox;


    public class EntityLiteScopedBusContextProvider<TBus, TDataService> :
        IScopedBusContextProvider<TBus>,
        IDisposable
        where TBus : class, IBus
        where TDataService : DataService
    {
        public EntityLiteScopedBusContextProvider(TBus bus, TDataService dbContext, IBusOutboxNotification notification,
            Bind<TBus, IClientFactory> clientFactory, Bind<TBus, IScopedConsumeContextProvider> consumeContextProvider,
            IScopedConsumeContextProvider globalConsumeContextProvider, IServiceProvider provider)
        {
            if (consumeContextProvider.Value.HasContext)
                Context = new ConsumeContextScopedBusContext(consumeContextProvider.Value.GetContext(), clientFactory.Value);
            else if (globalConsumeContextProvider.HasContext)
            {
                Context = new EntityLiteConsumeContextScopedBusContext<TBus, TDataService>(bus, dbContext, notification, clientFactory.Value, provider,
                    globalConsumeContextProvider.GetContext());
            }
            else
                Context = new EntityLiteScopedBusContext<TBus, TDataService>(bus, dbContext, notification, clientFactory.Value, provider);
        }

        public void Dispose()
        {
            switch (Context)
            {
                case IDisposable disposable:
                    disposable.Dispose();
                    return;
            }
        }

        public ScopedBusContext Context { get; }
    }
}
