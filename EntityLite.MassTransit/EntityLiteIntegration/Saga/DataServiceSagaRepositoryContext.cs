namespace MassTransit.EntityLiteIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using inercya.EntityLite;
    using Logging;
    using MassTransit.EntityLiteIntegration;
    using MassTransit.Saga;
    using Middleware;

#pragma warning disable CS8603 // Possible null reference return.
    public class DataServiceSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>,
        IDisposable
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly DataService dataService;
        readonly ISagaConsumeContextFactory<DataService, TSaga> _factory;
        readonly SemaphoreSlim _inUse = new SemaphoreSlim(1);
        readonly ISagaRepositoryLockStrategy<TSaga> _lockStrategy;

        public DataServiceSagaRepositoryContext(DataService dataService, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<DataService, TSaga> factory, ISagaRepositoryLockStrategy<TSaga> lockStrategy)
            : base(consumeContext, dataService)
        {
            this.dataService = dataService;
            _consumeContext = consumeContext;
            _factory = factory;
            _lockStrategy = lockStrategy;
        }

        public void Dispose()
        {
            _inUse.Dispose();
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(dataService, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                var repo = new Repository<TSaga>(dataService);
                await repo.InsertAsync(instance).ConfigureAwait(false);
                _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);
                return await _factory.CreateSagaConsumeContext(dataService, _consumeContext, instance, SagaConsumeContextMode.Insert)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _consumeContext.LogInsertFault<TSaga, TMessage>(ex, instance.CorrelationId);
                return default;
            }
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await _lockStrategy.Load(dataService, correlationId, CancellationToken).ConfigureAwait(false);
            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(dataService, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public async Task Save(SagaConsumeContext<TSaga> context)
        {
            await _inUse.WaitAsync(context.CancellationToken).ConfigureAwait(false);
            try
            {
                var repo = new Repository<TSaga>(dataService);
                await repo.InsertAsync(context.Saga).ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            await _inUse.WaitAsync(context.CancellationToken).ConfigureAwait(false);
            try
            {
                var repo = new Repository<TSaga>(dataService);
                await repo.UpdateAsync(context.Saga);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public async Task Delete(SagaConsumeContext<TSaga> context)
        {
            await _inUse.WaitAsync(context.CancellationToken).ConfigureAwait(false);
            try
            {
                var repo = new Repository<TSaga>(dataService);
                await repo.DeleteAsync(context.Saga);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(dataService, consumeContext, instance, mode);
        }
    }


    public class DataServiceSagaRepositoryContext<TSaga> :
        BasePipeContext,
        QuerySagaRepositoryContext<TSaga>,
        LoadSagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly DataService dataService;

        public DataServiceSagaRepositoryContext(DataService dataService, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            this.dataService = dataService;
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            return await new QueryLite<TSaga>(Projection.BaseTable, dataService)
                .Where(nameof(ISaga.CorrelationId), OperatorLite.Equals, correlationId)
                .FirstOrDefaultAsync();
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            var sagas = await new QueryLite<TSaga>(Projection.BaseTable, dataService)
                .Fields(FieldsOption.None, nameof(ISaga.CorrelationId))
                .Where(FilterLiteHelper.GetFilterFromExpression(query.FilterExpression))
                .ToEnumerableAsync();

            var results = sagas.Select(x => x.CorrelationId).ToList();

            return new DefaultSagaRepositoryQueryContext<TSaga>(this, results);
        }
    }
#pragma warning restore CS8603 // Possible null reference return.
}
