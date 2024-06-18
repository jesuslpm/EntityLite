namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using inercya.EntityLite;
    using MassTransit.EntityLiteIntegration;
    using MassTransit.EntityLiteIntegration.Saga;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Saga;


    public class EntityLiteSagaRepositoryConfigurator<TSaga> :
        IEntityLiteSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        ConcurrencyMode _concurrencyMode;
        Action<ISagaRepositoryRegistrationConfigurator<TSaga>>? _configureDataService;
        IsolationLevel _isolationLevel;
        ILockStatementProvider _lockStatementProvider;
        Func<IQueryLite<TSaga>, IQueryLite<TSaga>>? _queryCustomization;

        public EntityLiteSagaRepositoryConfigurator()
        {
            _isolationLevel = IsolationLevel.Serializable;
            _concurrencyMode = ConcurrencyMode.Pessimistic;
            _lockStatementProvider = new SqlServerLockStatementProvider();
        }

        public IsolationLevel IsolationLevel
        {
            set => _isolationLevel = value;
        }

        public void CustomizeQuery(Func<IQueryLite<TSaga>, IQueryLite<TSaga>> queryCustomization)
        {
            _queryCustomization = queryCustomization;
        }

        public ConcurrencyMode ConcurrencyMode
        {
            set
            {
                _concurrencyMode = value;
                if (_concurrencyMode == ConcurrencyMode.Optimistic && _isolationLevel == IsolationLevel.Serializable)
                    _isolationLevel = IsolationLevel.ReadCommitted;
            }
        }

        public ILockStatementProvider LockStatementProvider
        {
            set => _lockStatementProvider = value;
        }


        public void ExistingDataService<TDataService>()
            where TDataService : DataService
        {
            _configureDataService = configurator =>
            {
                configurator.TryAddScoped<ISagaDataServiceFactory<TSaga>, ContainerSagaDataServiceFactory<TDataService, TSaga>>();
            };
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_configureDataService == null)
                yield return this.Failure("DataService", "must be specified");
        }

        public void Register(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            _configureDataService?.Invoke(configurator);

            if (_concurrencyMode == ConcurrencyMode.Optimistic)
                configurator.TryAddSingleton(provider => CreateOptimisticLockStrategy());
            else
                configurator.TryAddSingleton(provider => CreatePessimisticLockStrategy());

            configurator.RegisterLoadSagaRepository<TSaga, EntityLiteSagaRepositoryContextFactory<TSaga>>();
            configurator.RegisterQuerySagaRepository<TSaga, EntityLiteSagaRepositoryContextFactory<TSaga>>();
            configurator
                .RegisterSagaRepository<TSaga, DataService, SagaConsumeContextFactory<DataService, TSaga>, EntityLiteSagaRepositoryContextFactory<TSaga>>();
        }

        ISagaRepositoryLockStrategy<TSaga> CreateOptimisticLockStrategy()
        {
            var queryExecutor = new OptimisticLoadQueryExecutor<TSaga>(_queryCustomization);

            return new OptimisticSagaRepositoryLockStrategy<TSaga>(queryExecutor, _queryCustomization, _isolationLevel);
        }

        ISagaRepositoryLockStrategy<TSaga> CreatePessimisticLockStrategy()
        {
            var statementProvider = _lockStatementProvider ?? new SqlServerLockStatementProvider();

            var queryExecutor = new PessimisticLoadQueryExecutor<TSaga>(statementProvider, _queryCustomization);

            return new PessimisticSagaRepositoryLockStrategy<TSaga>(queryExecutor, _isolationLevel);
        }

    }
}
