namespace MassTransit.EntityLiteIntegration
{
    using inercya.EntityLite;
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface ILoadQueryExecutor<TSaga>
        where TSaga : class, ISaga
    {
        Task<TSaga> Load(DataService ds, Guid correlationId, CancellationToken cancellationToken);
    }
}
