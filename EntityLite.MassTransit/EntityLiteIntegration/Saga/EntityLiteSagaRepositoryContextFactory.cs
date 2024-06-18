namespace MassTransit.EntityLiteIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using inercya.EntityLite;
    using MassTransit.Saga;


    public class EntityLiteSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>,
        IQuerySagaRepositoryContextFactory<TSaga>,
        ILoadSagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaConsumeContextFactory<DataService, TSaga> _consumeContextFactory;
        readonly ISagaDataServiceFactory<TSaga> dataServiceFactory;
        readonly ISagaRepositoryLockStrategy<TSaga> _lockStrategy;

        public EntityLiteSagaRepositoryContextFactory(ISagaDataServiceFactory<TSaga> dataServiceFactory,
            ISagaConsumeContextFactory<DataService, TSaga> consumeContextFactory, ISagaRepositoryLockStrategy<TSaga> lockStrategy)
        {
            this.dataServiceFactory = dataServiceFactory;
            _consumeContextFactory = consumeContextFactory;
            _lockStrategy = lockStrategy;
        }

        public Task<T> Execute<T>(Func<LoadSagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteAsyncMethod(asyncMethod, cancellationToken);
        }

        public Task<T> Execute<T>(Func<QuerySagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteAsyncMethod(asyncMethod, cancellationToken);
        }
        async Task<T> ExecuteAsyncMethod<T>(Func<DataServiceSagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
    where T : class
        {
            var ds = dataServiceFactory.Create();
            try
            {
                Task<T> ExecuteAsync()
                {
                    return WithinTransaction(ds, cancellationToken, () =>
                    {
                        var sagaRepositoryContext = new DataServiceSagaRepositoryContext<TSaga>(ds, cancellationToken);

                        return asyncMethod(sagaRepositoryContext);
                    });
                }
                return await ExecuteAsync().ConfigureAwait(false);
            }
            finally
            {
                await dataServiceFactory.ReleaseAsync(ds).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            var dbContext = dataServiceFactory.Create();
            try
            {
                context.Add("persistence", "EntityLite");
                //TODO: Add more information to the probe
                context.Add("entities", Array.Empty<string>());
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var ds = dataServiceFactory.CreateScoped(context);
            try
            {
                async Task SendAsyncCallback()
                {
                    using var repositoryContext = new DataServiceSagaRepositoryContext<TSaga, T>(ds, context, _consumeContextFactory, _lockStrategy);

                    await next.Send(repositoryContext).ConfigureAwait(false);
                }

                if (ds.IsActiveTransaction)
                    await SendAsyncCallback().ConfigureAwait(false);
                else
                {
                    await WithinTransaction(ds, context.CancellationToken, SendAsyncCallback).ConfigureAwait(false);
                }
            }
            finally
            {
                await dataServiceFactory.ReleaseAsync(ds).ConfigureAwait(false);
            }
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            var ds = dataServiceFactory.CreateScoped(context);
            try
            {
                async Task SendQueryAsyncCallback(SagaLockContext<TSaga> lockContext, SagaRepositoryContext<TSaga, T> repositoryContext)
                {
                    IList<TSaga> instances = await lockContext.Load().ConfigureAwait(false);

                    var queryContext = new LoadedSagaRepositoryQueryContext<TSaga, T>(repositoryContext, instances);

                    await next.Send(queryContext).ConfigureAwait(false);
                }

                async Task SendQueryAsync()
                {
                    SagaLockContext<TSaga> lockContext =
                        await _lockStrategy.CreateLockContext(ds, query, context.CancellationToken).ConfigureAwait(false);

                    using var repositoryContext = new DataServiceSagaRepositoryContext<TSaga, T>(ds, context, _consumeContextFactory, _lockStrategy);

                    if (ds.IsActiveTransaction)
                        await SendQueryAsyncCallback(lockContext, repositoryContext).ConfigureAwait(false);
                    else
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        await WithinTransaction(ds, context.CancellationToken, () => SendQueryAsyncCallback(lockContext, repositoryContext))
                            .ConfigureAwait(false);
                    }
                }
                await SendQueryAsync().ConfigureAwait(false);
            }
            finally
            {
                await dataServiceFactory.ReleaseAsync(ds).ConfigureAwait(false);
            }
        }



        Task WithinTransaction(DataService context, CancellationToken cancellationToken, Func<Task> callback)
        {
            async Task<bool> Create()
            {
                await callback().ConfigureAwait(false);
                return true;
            }

            return WithinTransaction(context, cancellationToken, Create);
        }

        async Task<T> WithinTransaction<T>(DataService ds, CancellationToken cancellationToken, Func<Task<T>> callback)
        {
            ds.BeginTransaction(_lockStrategy.IsolationLevel);

            static void Rollback(DataService ds)
            {
                try
                {
                    ds.Rollback();
                }
                catch (Exception innerException)
                {
                    LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                }
            }

            try
            {
                var result = await callback().ConfigureAwait(false);

                ds.Commit();

                return result;
            }
            catch (Exception)
            {
                Rollback(ds);
                throw;
            }
        }
    }
}
