using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using inercya.EntityLite;

namespace inercya.EntityLite.SqliteProfiler.Entities
{
    public partial class SqliteProfilerDataService 
    {

        public string FilePath { get; private set; }

        private static Regex listOfValuesRegex = new Regex(@"\((?:(?:[:@]P)*\d+(?:,\s*(?:[:@]P)*\d+)*)\)", RegexOptions.Compiled);

        private static Regex guidRegex = new Regex("([0-9A-Fa-f]{32})|([0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12})", RegexOptions.Compiled);


        public static SqliteProfilerDataService Create(string filePath)
        {
            //string connectionString = string.Format("Data Source=\"{0}\";journal mode=Off;page size=4096;cache size=1024;synchronous=Off", filePath);
            string connectionString = string.Format(CultureInfo.InvariantCulture, "Data Source=\"{0}\"", filePath);
            var ds = new SqliteProfilerDataService(connectionString);
            ds.FilePath = filePath;
            ds.CommandTimeout = 8;
            ds.IsPreventingSuperfluousUpdatesEnabled = false;
            return ds;
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
                if (IsOdlSchema()) UpgradeSchema();
                EnableWalMode();
            }
        }

        private void EnableWalMode()
        {
            string journalMode = null;
            using (var getJournalModeCmd = this.Connection.CreateCommand())
            {
                getJournalModeCmd.CommandText = "PRAGMA journal_mode;";
                journalMode = Convert.ToString(getJournalModeCmd.ExecuteScalar(), CultureInfo.InvariantCulture);
            }
            if (journalMode != "wal")
            {
                using (var turnOffJournalmodeCmd = this.Connection.CreateCommand())
                {
                    turnOffJournalmodeCmd.CommandText = "PRAGMA journal_mode=wal;";
                    turnOffJournalmodeCmd.ExecuteNonQuery();
                }
            }
        }
        private bool IsOdlSchema()
        {
            try
            {
                return this.SchemaVersionRepository.Query(inercya.EntityLite.Projection.BaseTable).FirstOrDefault().Version != "1.0.0";
            }
            catch
            {
                return true;
            }
        }

        private void UpgradeSchema()
        {
            this.Connection.Close();
            CreateProfileDatabase();
            base.OpenConnection();
        }


        private  void CreateProfileDatabase()
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("inercya.EntityLite.SqliteProfiler.EntityLiteProfile.db"))
            using (var file = File.Create(this.FilePath))
            {
                stream.CopyTo(file);
            }
        }

#pragma warning disable CA5351 // Not used for Cryptography, It is used for an index in SQLite database.
        private static readonly MD5 md5 = MD5.Create();
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms

        public void LogCommandExecution(LogItem item, bool fullLogging)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var normalizedCommandText = listOfValuesRegex.Replace(item.CommandText, "(#ListOfValues#)");
            normalizedCommandText = guidRegex.Replace(normalizedCommandText, "#Guid#");
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(normalizedCommandText);
            byte[] hash = md5.ComputeHash(inputBytes);
            long commandTextHash = BitConverter.ToInt64(hash, 0);
            var statement = this.StatementRepository.Query(Projection.BaseTable)
                .Where(nameof(Statement.CommandTextHash), OperatorLite.Equals, commandTextHash)
                .And(nameof(Statement.CommandText), normalizedCommandText).ToList().FirstOrDefault();

            string parametersAsString = item.Params;
            if (statement == null)
            {
                statement = new Statement();
                statement.CommandText = normalizedCommandText;
                statement.CommandTextHash = commandTextHash;
                statement.SampleCommandText = item.CommandText;
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
                    statement.SampleCommandText = item.CommandText;
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
