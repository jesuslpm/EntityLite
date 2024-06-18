using inercya.EntityLite;

namespace MassTransit.EntityLiteIntegration
{


    public interface ILockStatementProvider
    {
        string GetRowLockStatement<T>(DataService dataService)
            where T : class;

        /// <summary>
        /// Returns the lock statement for the specified property (usable for any entity)
        /// </summary>
        /// <param name="dataService"></param>
        /// <param name="propertyNames">One or more property names</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetRowLockStatement<T>(DataService dataService, params string[] propertyNames)
            where T : class;

        string GetOutboxStatement(DataService dataService);

        IQueryLite<OutboxState> GetOutboxQuery(DataService ds);

        IQueryLite<InboxState> GetInboxQuery(DataService ds);

        IQueryLite<T> GetLockQuery<T>(DataService ds) where T : class;
    }
}
