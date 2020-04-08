using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EntityLite.RavenDbProfiler
{
    internal class ProfilerRecord
    {
        public string CommandText;
        public CommandType CommandType;
        public TimeSpan ExecutionDelay;
        public DateTime ExecutionDate;
        public object UserId;
        public string Context;
        public Dictionary<string, object> Parameters;
    }

    public class NormalizedSqlStatement
    {
        public string Id { get; set; }
        public string Sql { get; set; }
    }

    public class SqlStatement
    {
        public string Id { get; set; }
        public string Sql { get; set; }
        public string NormalizedSqlStatementId { get; set; }
    }

    public class SqlStatementExecution
    {
        public string Id { get; set; }
        public string SqlStatementId { get; set; }
        public string NormalizedSqlStatementId { get; set; }
        public CommandType CommandType { get; set; }
        public TimeSpan ExecutionDelay { get; set; }
        public DateTime ExecutionDate { get; set; }
        public object UserId { get; set; }
        public string Context { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
