using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using System.Data.Common;
using inercya.EntityLite.SqliteProfiler;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace inercya.EntityLite.SqliteProfiler.Entities
{
    public partial class SqliteProfilerDataService 
    {

        public string FilePath { get; private set; }


        public static SqliteProfilerDataService Create(string filePath)
        {
            //string connectionString = string.Format("Data Source=\"{0}\";journal mode=Off;page size=4096;cache size=1024;synchronous=Off", filePath);
            //TODO: turn off journal mode.
            string connectionString = string.Format("Data Source=\"{0}\"", filePath);
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
                using (var cmd = this.Connection.CreateCommand())
                {
                    cmd.CommandText = "PRAGMA journal_mode = OFF;";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private  void CreateProfileDatabase()
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("inercya.EntityLite.SqliteProfiler.EntityLiteProfile.db"))
            using (var file = File.Create(this.FilePath))
            {
#if (NET452 || NETSTANDARD2_0)
                stream.CopyTo(file);
#else
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                file.Write(buffer, 0, buffer.Length);
#endif
            }
        }

        private static readonly MD5 md5 = System.Security.Cryptography.MD5.Create();

        public void LogCommandExecution(LogItem item, bool fullLogging)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(item.CommandText);
            byte[] hash = md5.ComputeHash(inputBytes);
            long commandTextHash = BitConverter.ToInt64(hash, 0);
            var statement = this.StatementRepository.Query(Projection.BaseTable)
                .Where(nameof(Statement.CommandTextHash), commandTextHash)
                .And(nameof(Statement.CommandText), item.CommandText).ToList().FirstOrDefault();

            string parametersAsString = item.Params;
            if (statement == null)
            {
                statement = new Statement();
                statement.CommandText = item.CommandText;
                statement.CommandTextHash = commandTextHash;
                statement.ExecutionCount = 1;
                statement.MaxTime = item.ExecutionTime.TotalMilliseconds;
                statement.MaxTimeParams = parametersAsString;
                statement.MinTime = item.ExecutionTime.TotalMilliseconds;
                statement.MinTimeParams = parametersAsString;
                statement.SampleParams = parametersAsString;
                statement.SampleTime = item.ExecutionTime.TotalMilliseconds;
                statement.TotalTime = item.ExecutionTime.TotalMilliseconds;
                this.Insert(statement);
            }
            else
            {
                statement.ExecutionCount += 1;
                statement.TotalTime += item.ExecutionTime.TotalMilliseconds;


                if (item.ExecutionTime.TotalMilliseconds > statement.MaxTime)
                {
                    statement.MaxTime = item.ExecutionTime.TotalMilliseconds;
                    statement.MaxTimeParams = parametersAsString;
                }
                if (item.ExecutionTime.TotalMilliseconds < statement.MinTime)
                {
                    statement.MinTime = item.ExecutionTime.TotalMilliseconds;
                    statement.MinTimeParams = parametersAsString;
                }

                double avgTime = statement.TotalTime / statement.ExecutionCount;

                if (Math.Abs(avgTime - item.ExecutionTime.TotalMilliseconds) < Math.Abs(avgTime - statement.SampleTime))
                {
                    statement.SampleTime = item.ExecutionTime.TotalMilliseconds;
                    statement.SampleParams = parametersAsString;
                }

                this.Update(statement);
            }

            if (fullLogging)
            {
                var exec = new Execution();
                exec.ExecutionDate = DateTime.Now;
                exec.ExecutionTime = item.ExecutionTime.TotalMilliseconds;
                exec.StatementId = statement.StatementId;
                exec.DataServiceInstanceId = item.DataServiceInstanceId.ToString("N");
                exec.ApplicationContext = item.ApplicationContext;
                this.Insert(exec);
            }
        }
    }
}
