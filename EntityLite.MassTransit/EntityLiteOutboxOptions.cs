namespace MassTransit
{
    using System.Data;
    using EntityLiteIntegration;


    public class EntityLiteOutboxOptions
    {
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.RepeatableRead;
        public ILockStatementProvider LockStatementProvider { get; set; } = new SqlServerLockStatementProvider();
    }
}
