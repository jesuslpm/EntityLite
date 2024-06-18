namespace MassTransit.EntityLiteIntegration.Saga
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using inercya.EntityLite;
    using MassTransit.EntityLiteIntegration;


    public class PessimisticLoadQueryExecutor<TSaga> :
        ILoadQueryExecutor<TSaga>
        where TSaga : class, ISaga
    {
        readonly ILockStatementProvider _lockStatementProvider;
        readonly Func<IQueryLite<TSaga>, IQueryLite<TSaga>>? _queryCustomization;

        public PessimisticLoadQueryExecutor(ILockStatementProvider lockStatementProvider, Func<IQueryLite<TSaga>, IQueryLite<TSaga>>? queryCustomization)
        {
            _lockStatementProvider = lockStatementProvider;
            _queryCustomization = queryCustomization;
        }

        public async Task<TSaga> Load(DataService ds, Guid correlationId, CancellationToken cancellationToken)
        {
            var query = _lockStatementProvider.GetLockQuery<TSaga>(ds)
                .Where(nameof(ISaga.CorrelationId), OperatorLite.Equals, correlationId);

            if (_queryCustomization != null)
                query = _queryCustomization(query);

            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }

    }
}
