namespace MassTransit.EntityLiteIntegration.Saga
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.EntityLiteIntegration;
    using inercya.EntityLite;

    public class OptimisticSagaRepositoryLockStrategy<TSaga> :
        ISagaRepositoryLockStrategy<TSaga>
        where TSaga : class, ISaga
    {
        readonly ILoadQueryExecutor<TSaga> _executor;
        readonly Func<IQueryLite<TSaga>, IQueryLite<TSaga>>? _queryCustomization;

        public OptimisticSagaRepositoryLockStrategy(ILoadQueryExecutor<TSaga> executor, Func<IQueryLite<TSaga>, IQueryLite<TSaga>>? queryCustomization,
            IsolationLevel isolationLevel)
        {
            _executor = executor;
            _queryCustomization = queryCustomization;

            IsolationLevel = isolationLevel;
        }

        public IsolationLevel IsolationLevel { get; }

        public Task<TSaga> Load(DataService ds, Guid correlationId, CancellationToken cancellationToken)
        {
            return _executor.Load(ds, correlationId, cancellationToken);
        }

        public Task<SagaLockContext<TSaga>> CreateLockContext(DataService ds, ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            SagaLockContext<TSaga> context = new OptimisticSagaLockContext<TSaga>(ds, query, cancellationToken, _queryCustomization);
            return Task.FromResult(context);
        }
    }
}
