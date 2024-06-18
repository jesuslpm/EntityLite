#nullable enable
namespace MassTransit.EntityLiteIntegration
{
    using System;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Clients;
    using DependencyInjection;
    using inercya.EntityLite;
    using Middleware;
    using Middleware.Outbox;
    using Serialization;
    using Transports;


    public class EntityLiteScopedBusContext<TBus, TDataService> :
        ScopedBusContext,
        OutboxSendContext,
        IDisposable
        where TBus : class, IBus
        where TDataService : DataService
    {
        readonly TBus _bus;
        readonly IClientFactory _clientFactory;
        readonly TDataService _ds;
        readonly IBusOutboxNotification _notification;
        readonly IServiceProvider _provider;
        readonly SemaphoreSlim _lock;
        Guid? _outboxId;
        IPublishEndpoint? _publishEndpoint;
        IScopedClientFactory? _scopedClientFactory;
        ISendEndpointProvider? _sendEndpointProvider;
        int _transactionCount;
        DbTransaction? _transaction;

        public EntityLiteScopedBusContext(TBus bus, TDataService ds, IBusOutboxNotification notification, IClientFactory clientFactory,
            IServiceProvider provider)
        {
            _bus = bus;
            _ds = ds;
            _notification = notification;
            _clientFactory = clientFactory;
            _provider = provider;
            _lock = new SemaphoreSlim(1);
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            _lock.Wait();
            if (WasCommitted())
                _notification.Delivered();

            _outboxId = null;
            _lock.Dispose();
        }

        public async Task AddSend<T>(SendContext<T> context)
            where T : class
        {

            if (_outboxId == null || WasCommitted())
            {
                await _lock.WaitAsync(context.CancellationToken).ConfigureAwait(false);
                try
                {
                    if (_ds.IsActiveTransaction == false)
                    {
                        throw new InvalidOperationException("The EntityLite Scoped Bus Context must be used within a transaction");
                    }

                    if (WasCommitted())
                    {
                        _notification.Delivered();
                        _outboxId = null;
                    }

                    _outboxId ??= await CreateOutboxState(context.CancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    _lock.Release();
                }
            }

            await _ds.AddSend(context, SystemTextJsonMessageSerializer.Instance, outboxId: _outboxId);
        }

        public object? GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider ??= new OutboxSendEndpointProvider(this, GetSendEndpointProvider());

        public IPublishEndpoint PublishEndpoint =>
            _publishEndpoint ??= new PublishEndpoint(new OutboxPublishEndpointProvider(this, GetPublishEndpointProvider()));

        public IScopedClientFactory ClientFactory => _scopedClientFactory ??= GetClientFactory();

        bool WasCommitted()
        {
            return _outboxId.HasValue && (_transaction != _ds.Transaction);
        }

        protected virtual ScopedClientFactory GetClientFactory()
        {
            return new ScopedClientFactory(new ClientFactory(new ScopedClientFactoryContext(_clientFactory, _provider)), null);
        }

        protected virtual IPublishEndpointProvider GetPublishEndpointProvider()
        {
            return _bus;
        }

        protected virtual ISendEndpointProvider GetSendEndpointProvider()
        {
            return _bus;
        }

        async Task<Guid?> CreateOutboxState(CancellationToken cancellationToken)
        {
            _transactionCount = _ds.TransactionCount;
            _transaction = _ds.Transaction;
            var outboxId = NewId.NextGuid();
            var outboxState = new OutboxState
            {
                OutboxId = outboxId,
                Created = DateTime.UtcNow
            };
            var repo = new Repository<OutboxState>(_ds);
            await repo.InsertAsync(outboxState).ConfigureAwait(false);

            return outboxId;
        }
    }
}
