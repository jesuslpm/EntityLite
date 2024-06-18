#nullable enable
namespace MassTransit.EntityLiteIntegration;

using System;
using Clients;
using DependencyInjection;
using inercya.EntityLite;
using Middleware.Outbox;


public class EntityLiteConsumeContextScopedBusContext<TBus, TDataService> :
    EntityLiteScopedBusContext<TBus, TDataService>
    where TBus : class, IBus
    where TDataService : DataService
{
    readonly TBus _bus;
    readonly IClientFactory _clientFactory;
    readonly ConsumeContext _consumeContext;
    readonly IServiceProvider _provider;

    public EntityLiteConsumeContextScopedBusContext(TBus bus, TDataService dbContext, IBusOutboxNotification notification, IClientFactory clientFactory,
        IServiceProvider provider, ConsumeContext consumeContext)
        : base(bus, dbContext, notification, clientFactory, provider)
    {
        _bus = bus;
        _clientFactory = clientFactory;
        _provider = provider;
        _consumeContext = consumeContext;
    }

    protected override IPublishEndpointProvider GetPublishEndpointProvider()
    {
        return new ScopedConsumePublishEndpointProvider(_bus, _consumeContext, _provider);
    }

    protected override ISendEndpointProvider GetSendEndpointProvider()
    {
        return new ScopedConsumeSendEndpointProvider(_bus, _consumeContext, _provider);
    }

    protected override ScopedClientFactory GetClientFactory()
    {
        return new ScopedClientFactory(new ClientFactory(new ScopedClientFactoryContext(_clientFactory, _provider)), _consumeContext);
    }
}
