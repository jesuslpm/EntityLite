using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace inercya.EntityLite.RavenDbProfiler
{
    internal class ProfilerRecord
    {
        public string CommandText;
        public CommandType CommandType;
        public TimeSpan ExecutionDelay;
        public DateTime ExecutionDate;
        public object UserId;
        public Guid DataServiceInstanceId;
        public SortedDictionary<string, string> Context;
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
        public double ExecutionMilliseconds { get; set; }
        public DateTime ExecutionDate { get; set; }
        public object UserId { get; set; }
        public SortedDictionary<string, string> Context { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public Guid DataServiceInstanceId { get; set; }
    }
}
