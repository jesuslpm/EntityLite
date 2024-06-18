namespace MassTransit.EntityLiteIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using inercya.EntityLite;
    using MassTransit.EntityLiteIntegration;


    public class PessimisticSagaRepositoryLockStrategy<TSaga> :
        ISagaRepositoryLockStrategy<TSaga>
        where TSaga : class, ISaga
    {
        readonly ILoadQueryExecutor<TSaga> _executor;

        public PessimisticSagaRepositoryLockStrategy(ILoadQueryExecutor<TSaga> executor, IsolationLevel isolationLevel)
        {
            _executor = executor;

            IsolationLevel = isolationLevel;
        }

        public IsolationLevel IsolationLevel { get; }

        public Task<TSaga> Load(DataService ds, Guid correlationId, CancellationToken cancellationToken)
        {
            return _executor.Load(ds, correlationId, cancellationToken);
        }

        public async Task<SagaLockContext<TSaga>> CreateLockContext(DataService ds, ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            //IList<Guid> instances = await ds.Set<TSaga>()
            //    .AsNoTracking()
            //    .Where(query.FilterExpression)
            //    .Select(x => x.CorrelationId)
            //    .ToListAsync(cancellationToken)
            //    .ConfigureAwait(false);

            var sagas = await new QueryLite<TSaga>(Projection.BaseTable, ds)
                .Fields(nameof(ISaga.CorrelationId))
                .Where(FilterLiteHelper.GetFilterFromExpression(query.FilterExpression))
                .ToEnumerableAsync();

            var instances = sagas.Select(x => x.CorrelationId).ToList();

            return new PessimisticSagaLockContext<TSaga>(ds, cancellationToken, instances, _executor);
        }
    }
}
