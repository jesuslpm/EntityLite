namespace MassTransit
{
    using System;
    using System.Data;
    using System.Linq;
    using inercya.EntityLite;
    using MassTransit.EntityLiteIntegration;


    public interface IEntityLiteSagaRepositoryConfigurator
    {
        ConcurrencyMode ConcurrencyMode { set; }
        IsolationLevel IsolationLevel { set; }
        ILockStatementProvider LockStatementProvider { set; }


        /// <summary>
        /// Use an existing (already configured in the container) DataService that will be resolved
        /// within the container scope
        /// </summary>
        /// <typeparam name="TDataService"></typeparam>
        void ExistingDataService<TDataService>()
            where TDataService : DataService;
    }


    public interface IEntityLiteSagaRepositoryConfigurator<TSaga> :
        IEntityLiteSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Use custom query
        /// </summary>
        /// <param name="queryCustomization"></param>
        void CustomizeQuery(Func<IQueryLite<TSaga>, IQueryLite<TSaga>> queryCustomization);
    }
}
