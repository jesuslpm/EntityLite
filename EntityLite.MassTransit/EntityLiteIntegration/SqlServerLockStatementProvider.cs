namespace MassTransit.EntityLiteIntegration
{
    public class SqlServerLockStatementProvider :
        SqlLockStatementProvider
    {
        public SqlServerLockStatementProvider(bool enableSchemaCaching = true, bool serializable = false)
            : base(new SqlServerLockStatementFormatter(serializable), enableSchemaCaching)
        {
        }
    }
}
