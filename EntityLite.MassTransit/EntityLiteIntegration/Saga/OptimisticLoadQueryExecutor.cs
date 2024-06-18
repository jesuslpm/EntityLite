namespace MassTransit.EntityLiteIntegration.Saga
{
    using inercya.EntityLite;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    public class OptimisticLoadQueryExecutor<TSaga> :
        ILoadQueryExecutor<TSaga>
        where TSaga : class, ISaga
    {
        readonly Func<IQueryLite<TSaga>, IQueryLite<TSaga>>? _queryCustomization;

        public OptimisticLoadQueryExecutor(Func<IQueryLite<TSaga>, IQueryLite<TSaga>>? queryCustomization = null)
        {
            _queryCustomization = queryCustomization;
        }

        public async Task<TSaga> Load(DataService ds, Guid correlationId, CancellationToken cancellationToken)
        {
            var query = new QueryLite<TSaga>(Projection.BaseTable, ds)
                    .Where(nameof(ISaga.CorrelationId), OperatorLite.Equals, correlationId);

            if (_queryCustomization == null)
            {
                return await query.FirstOrDefaultAsync();
            }

            return await _queryCustomization(query).FirstOrDefaultAsync();
            
        }
    }
}
