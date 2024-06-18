namespace MassTransit.EntityLiteIntegration
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using inercya.EntityLite;
    using Saga;


    public interface ISagaRepositoryLockStrategy<TSaga>
        where TSaga : class, ISaga
    {
        IsolationLevel IsolationLevel { get; }

        Task<TSaga> Load(DataService ds, Guid correlationId, CancellationToken cancellationToken);

        Task<SagaLockContext<TSaga>> CreateLockContext(DataService context, ISagaQuery<TSaga> query, CancellationToken cancellationToken);
    }
}
