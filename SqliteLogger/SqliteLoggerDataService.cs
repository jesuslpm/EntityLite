using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using System.Data.SQLite;
using System.Data.Common;
using inercya.EntityLite.SqliteProfiler;
using System.IO;

namespace inercya.EntityLite.SqliteProfiler.Entities
{
    public partial class SqliteLoggerDataService 
    {

        public string FileName { get; private set; }


        public static int GetWeekNumber(DateTime date)
        {              
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static string GetFileName()
        {
            DateTime today = DateTime.Today;
            return string.Format("LOG_{0:0000}_{1:00}.db", today.Year, GetWeekNumber(today));
        }

        public static SqliteLoggerDataService Create()
        {
            var fileName = GetFileName();
            var logger = new SqliteLoggerDataService(GetConnectionString(true, fileName), "System.Data.SQLite");
            logger.FileName = fileName;
            return logger;
        }


        private static string GetConnectionString(bool failIfMissing, string fileName)
        {
            var builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            builder.DateTimeFormat = SQLiteDateFormats.ISO8601;
            builder.FailIfMissing = failIfMissing;
            builder.JournalMode = SQLiteJournalModeEnum.Off;
            builder.PageSize = 4096;
            builder.CacheSize = 1024;
            builder.SyncMode = SynchronizationModes.Off;
            return builder.ConnectionString;
        }

        public override void OpenConnection()
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                bool isNewFile = false;
                if (!File.Exists(this.FileName))
                {
                    SQLiteConnection.CreateFile(this.FileName);
                    isNewFile = true;
                }
                base.OpenConnection();
                if (isNewFile)
                {
                    CreateDatabaseObjects();
                }
            }
        }

        private void CreateDatabaseObjects()
        {
            using (var cmd = this.Connection.CreateCommand())
            {
                this.BeginTransaction();
                cmd.Transaction = this.Transaction;
                cmd.CommandText = Resources.CreateDatabaseObjects;
                cmd.ExecuteNonQuery();
                this.Commit();
            }
        }


        public void LogCommandExecution(LogItem item, bool onlyStatement)
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

            if (!onlyStatement)
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
