namespace MassTransit.EntityLiteIntegration
{
    using inercya.EntityLite;
    using System.Threading.Tasks;


    /// <summary>
    /// Creates a DbContext for the saga repository
    /// </summary>
    public interface ISagaDataServiceFactory<out TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Create a standalone DataService
        /// </summary>
        /// <returns></returns>
        DataService Create();

        /// <summary>
        /// Create a scoped DbContext within the lifetime scope of the saga repository
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        DataService CreateScoped<T>(ConsumeContext<T> context)
            where T : class;

        /// <summary>
        /// Release the DataService once it is no longer needed
        /// </summary>
        /// <param name="ds"></param>
        ValueTask ReleaseAsync(DataService ds);
    }
}
