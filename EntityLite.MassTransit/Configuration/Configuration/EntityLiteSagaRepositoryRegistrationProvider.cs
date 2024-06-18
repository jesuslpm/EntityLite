namespace MassTransit.Configuration
{
    using System;


    public class EntityLiteSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly Action<IEntityLiteSagaRepositoryConfigurator>? _configure;

        public EntityLiteSagaRepositoryRegistrationProvider(Action<IEntityLiteSagaRepositoryConfigurator>? configure)
        {
            _configure = configure;
        }

        public void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            configurator.EntityLiteRepository(r => _configure?.Invoke(r));
        }


    }
}
