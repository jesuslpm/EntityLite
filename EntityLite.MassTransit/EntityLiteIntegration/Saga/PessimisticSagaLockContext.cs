namespace MassTransit.EntityLiteIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.EntityLiteIntegration;
    using inercya.EntityLite;


    /// <summary>
    /// Queries the list of saga ids prior to the transaction, and then loads/locks them individually
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class PessimisticSagaLockContext<TSaga> :
        SagaLockContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly CancellationToken _cancellationToken;
        readonly DataService ds;
        readonly ILoadQueryExecutor<TSaga> _executor;
        readonly IList<Guid> _instances;

        public PessimisticSagaLockContext(DataService ds, CancellationToken cancellationToken, IList<Guid> instances, ILoadQueryExecutor<TSaga> executor)
        {
            this.ds = ds;
            _cancellationToken = cancellationToken;
            _instances = instances;
            _executor = executor;
        }

        public async Task<IList<TSaga>> Load()
        {
            var loaded = new List<TSaga>();

            //TODO: This could be optimized to load all at once
            foreach (var correlationId in _instances)
            {
                var result = await _executor.Load(ds, correlationId, _cancellationToken).ConfigureAwait(false);
                if (result != null)
                    loaded.Add(result);
            }

            return loaded;
        }
    }
}
