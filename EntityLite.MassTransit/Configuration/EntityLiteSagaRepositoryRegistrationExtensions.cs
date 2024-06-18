namespace MassTransit
{
    using System;
    using System.Reflection.Emit;
    using Configuration;
    using MassTransit.EntityLiteIntegration;


    public static class EntityLiteSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a EntityLite saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> EntityLiteRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Action<IEntityLiteSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            var repositoryConfigurator = new EntityLiteSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The EntityLite saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        ///// <summary>
        ///// Adds a EntityFramework saga repository to the registration
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <param name="sagaRepository"></param>
        ///// <param name="configure"></param>
        ///// <param name="configureSagaMapping"></param>
        ///// <typeparam name="TSaga"></typeparam>
        ///// <returns></returns>
        //public static ISagaRegistrationConfigurator<TSaga> EntityFrameworkRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
        //    IEntityFrameworkSagaRepository sagaRepository, Action<IEntityFrameworkSagaRepositoryConfigurator<TSaga>> configure = null,
        //    Action<EntityTypeBuilder<TSaga>> configureSagaMapping = null)
        //    where TSaga : class, ISaga
        //{
        //    return configurator.EntityFrameworkRepository(sagaRepository, configure, new ActionSagaClassMap<TSaga>(configureSagaMapping));
        //}

        ///// <summary>
        ///// Adds a EntityFramework saga repository to the registration
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <param name="sagaRepository"></param>
        ///// <param name="configure"></param>
        ///// <param name="sagaClassMap"></param>
        ///// <typeparam name="TSaga"></typeparam>
        ///// <returns></returns>
        //public static ISagaRegistrationConfigurator<TSaga> EntityFrameworkRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
        //    IEntityFrameworkSagaRepository sagaRepository, Action<IEntityFrameworkSagaRepositoryConfigurator<TSaga>> configure = null,
        //    ISagaClassMap<TSaga> sagaClassMap = null)
        //    where TSaga : class, ISaga
        //{
        //    sagaRepository.AddSagaClassMap(sagaClassMap ?? new ActionSagaClassMap<TSaga>());
        //    return configurator.EntityFrameworkRepository(cfg =>
        //    {
        //        cfg.DatabaseFactory(sagaRepository.GetDbContext);

        //        configure?.Invoke(cfg);
        //    });
        //}

        ///// <summary>
        ///// Configure the Job Service saga state machines to use Entity Framework Core as the saga repository
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <param name="configure"></param>
        ///// <returns></returns>
        //public static IJobSagaRegistrationConfigurator EntityFrameworkRepository(this IJobSagaRegistrationConfigurator configurator,
        //    Action<IEntityFrameworkSagaRepositoryConfigurator> configure = null)
        //{
        //    var registrationProvider = new EntityFrameworkSagaRepositoryRegistrationProvider(configure);

        //    configurator.UseRepositoryRegistrationProvider(registrationProvider);

        //    return configurator;
        //}

        /// <summary>
        /// Use the EntityLite saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void SetEntityFrameworkSagaRepositoryProvider(this IRegistrationConfigurator configurator,
            Action<IEntityLiteSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new EntityLiteSagaRepositoryRegistrationProvider(configure));
        }

        /// <summary>
        /// Configure the repository for use with SQL Server
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEntityLiteSagaRepositoryConfigurator<T> UseSqlServer<T>(this IEntityLiteSagaRepositoryConfigurator<T> configurator)
            where T : class, ISaga
        {
            configurator.LockStatementProvider = new SqlServerLockStatementProvider();

            return configurator;
        }

        ///// <summary>
        ///// Configure the repository for use with SQL Server
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <param name="schemaName">The schema name to use if the table schema cannot be discovered</param>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator<T> UseSqlServer<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator,
        //    string schemaName)
        //    where T : class, ISaga
        //{
        //    if (schemaName == null)
        //        throw new ArgumentNullException(nameof(schemaName));

        //    configurator.LockStatementProvider = new SqlServerLockStatementProvider(schemaName);

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the repository for use with SQL Server
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator UseSqlServer(this IEntityFrameworkSagaRepositoryConfigurator configurator)
        //{
        //    configurator.LockStatementProvider = new SqlServerLockStatementProvider();

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the repository for use with SQL Server
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <param name="schemaName">The schema name to use if the table schema cannot be discovered</param>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator UseSqlServer(this IEntityFrameworkSagaRepositoryConfigurator configurator,
        //    string schemaName)
        //{
        //    if (schemaName == null)
        //        throw new ArgumentNullException(nameof(schemaName));

        //    configurator.LockStatementProvider = new SqlServerLockStatementProvider(schemaName);

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the repository for use with Postgres
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator<T> UsePostgres<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator)
        //    where T : class, ISaga
        //{
        //    configurator.LockStatementProvider = new PostgresLockStatementProvider();

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the repository for use with Postgres
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <param name="schemaName">The schema name to use if the table schema cannot be discovered</param>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator<T> UsePostgres<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator,
        //    string schemaName)
        //    where T : class, ISaga
        //{
        //    if (schemaName == null)
        //        throw new ArgumentNullException(nameof(schemaName));

        //    configurator.LockStatementProvider = new PostgresLockStatementProvider(schemaName);

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the repository for use with Postgres
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator UsePostgres(this IEntityFrameworkSagaRepositoryConfigurator configurator)
        //{
        //    configurator.LockStatementProvider = new PostgresLockStatementProvider();

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the repository for use with Postgres
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <param name="schemaName">The schema name to use if the table schema cannot be discovered</param>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator UsePostgres(this IEntityFrameworkSagaRepositoryConfigurator configurator,
        //    string schemaName)
        //{
        //    if (schemaName == null)
        //        throw new ArgumentNullException(nameof(schemaName));

        //    configurator.LockStatementProvider = new PostgresLockStatementProvider(schemaName);

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the repository for use with MySQL
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator<T> UseMySql<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator)
        //    where T : class, ISaga
        //{
        //    configurator.LockStatementProvider = new MySqlLockStatementProvider();

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the repository for use with MySQL
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator UseMySql(this IEntityFrameworkSagaRepositoryConfigurator configurator)
        //{
        //    configurator.LockStatementProvider = new MySqlLockStatementProvider();

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the repository for use with SQLite
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator<T> UseSqlite<T>(this IEntityFrameworkSagaRepositoryConfigurator<T> configurator)
        //    where T : class, ISaga
        //{
        //    configurator.LockStatementProvider = new SqliteLockStatementProvider();

        //    return configurator;
        //}

        ///// <summary>
        ///// Configure the repository for use with SQLite
        ///// </summary>
        ///// <param name="configurator"></param>
        ///// <returns></returns>
        //public static IEntityFrameworkSagaRepositoryConfigurator UseSqlite(this IEntityFrameworkSagaRepositoryConfigurator configurator)
        //{
        //    configurator.LockStatementProvider = new SqliteLockStatementProvider();

        //    return configurator;
        //}



        //class ActionSagaClassMap<T> : SagaClassMap<T>
        //    where T : class, ISaga
        //{
        //    readonly Action<EntityTypeBuilder<T>> _configure;

        //    public ActionSagaClassMap(Action<EntityTypeBuilder<T>> configure = null)
        //    {
        //        _configure = configure;
        //    }

        //    protected override void Configure(EntityTypeBuilder<T> entity, ModelBuilder model)
        //    {
        //        base.Configure(entity, model);
        //        _configure?.Invoke(entity);
        //    }
        //}
    }
}
