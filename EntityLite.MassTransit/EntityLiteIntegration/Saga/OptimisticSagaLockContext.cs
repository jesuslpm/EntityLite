namespace MassTransit.EntityLiteIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using inercya.EntityLite;
    using MassTransit.EntityLiteIntegration.Saga;


    /// <summary>
    /// Defers loading the sagas until the transaction is started
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class OptimisticSagaLockContext<TSaga> :
        SagaLockContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly CancellationToken _cancellationToken;
        readonly DataService _context;
        readonly ISagaQuery<TSaga> _query;
        readonly Func<IQueryLite<TSaga>, IQueryLite<TSaga>>? _queryCustomization;

        public OptimisticSagaLockContext(DataService context, ISagaQuery<TSaga> query, CancellationToken cancellationToken,
            Func<IQueryLite<TSaga>, IQueryLite<TSaga>>? queryCustomization)
        {
            _context = context;
            _query = query;
            _cancellationToken = cancellationToken;
            _queryCustomization = queryCustomization;
        }

        public async Task<IList<TSaga>> Load()
        {
            IQueryLite<TSaga> queryLite  = new QueryLite<TSaga>(Projection.BaseTable, _context);
            if (_queryCustomization != null)
                queryLite = _queryCustomization(queryLite);

            var instances = await queryLite
                .Where(FilterLiteHelper.GetFilterFromExpression(_query.FilterExpression))
                .ToListAsync()
                .ConfigureAwait(false);

            return instances;
        }
    }
}
