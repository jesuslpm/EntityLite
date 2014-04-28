using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using System.Data.Common;
using inercya.EntityLite.SqliteProfiler;
using System.IO;

namespace inercya.EntityLite.SqliteProfiler.Entities
{
    public partial class SqliteProfilerDataService 
    {

        public string FilePath { get; private set; }


        public static SqliteProfilerDataService Create(string filePath)
        {
            string connectionString = string.Format("Data Source=\"{0}\";journal mode=Off;page size=4096;cache size=1024;synchronous=Off", filePath);
            var logger = new SqliteProfilerDataService(connectionString, "System.Data.SQLite");
            logger.FilePath = filePath;
            return logger;
        }


        public override void OpenConnection()
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                if (!File.Exists(this.FilePath))
                {
                    CreateProfileDatabase();
                }
                base.OpenConnection();
            }
        }

        private  void CreateProfileDatabase()
        {
            using (var file = File.Create(this.FilePath))
            {
                file.Write(Resources.EntityLiteProfile, 0, Resources.EntityLiteProfile.Length);
            }
        }


        public void LogCommandExecution(LogItem item, bool fullLogging)
        {
            long commandTextHash = item.CommandText.GetHashCode();
            var statement = this.StatementRepository.Query(Projection.BaseTable)
                .Where(StatementFields.CommandTextHash, commandTextHash)
                .And(StatementFields.CommandText, item.CommandText).ToList().FirstOrDefault();

            string parametersAsString = item.Params;
            if (statement == null)
            {
                statement = new Statement();
                statement.CommandText = item.CommandText;
                statement.CommandTextHash = commandTextHash;
                statement.ExecutionCount = 1;
                statement.MaxTime = item.ExecutionTime;
                statement.MaxTimeParams = parametersAsString;
                statement.MinTime = item.ExecutionTime;
                statement.MinTimeParams = parametersAsString;
                statement.SampleParams = parametersAsString;
                statement.SampleTime = item.ExecutionTime;
                statement.TotalTime = item.ExecutionTime;
                this.Insert(statement);
            }
            else
            {
                statement.ExecutionCount += 1;
                statement.TotalTime += item.ExecutionTime;


                if (item.ExecutionTime > statement.MaxTime)
                {
                    statement.MaxTime = item.ExecutionTime;
                    statement.MaxTimeParams = parametersAsString;
                }
                if (item.ExecutionTime < statement.MinTime)
                {
                    statement.MinTime = item.ExecutionTime;
                    statement.MinTimeParams = parametersAsString;
                }

                double avgTime = statement.TotalTime / statement.ExecutionCount;

                if (Math.Abs(avgTime - item.ExecutionTime) < Math.Abs(avgTime - statement.SampleTime))
                {
                    statement.SampleTime = item.ExecutionTime;
                    statement.SampleParams = parametersAsString;
                }

                this.Update(statement);
            }

            if (fullLogging)
            {
                var exec = new Execution();
                exec.ExecutionDate = DateTime.Now;
                exec.ExecutionTime = item.ExecutionTime;
                exec.StatementId = statement.StatementId;
                this.Insert(exec);
            }
        }
    }
}
