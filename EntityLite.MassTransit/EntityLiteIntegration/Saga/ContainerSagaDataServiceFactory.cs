namespace MassTransit.EntityLiteIntegration.Saga
{
    using inercya.EntityLite;
    using System.Threading.Tasks;


    public class ContainerSagaDataServiceFactory<TDataService, TSaga> :
        ISagaDataServiceFactory<TSaga>
        where TDataService : DataService
        where TSaga : class, ISaga
    {
        readonly TDataService _dbContext;

        public ContainerSagaDataServiceFactory(TDataService dbContext)
        {
            _dbContext = dbContext;
        }

        public DataService Create()
        {
            return _dbContext;
        }

        public DataService CreateScoped<T>(ConsumeContext<T> context)
            where T : class
        {
            return _dbContext;
        }

        public ValueTask ReleaseAsync(DataService dbContext)
        {
            return default;
        }
    }
}
