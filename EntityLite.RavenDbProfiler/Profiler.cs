using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using inercya.EntityLite;
using Microsoft.Extensions.Logging;
using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.BulkInsert;
using Raven.Client.Documents.Session;
using Raven.Client.Json;

namespace EntityLite.RavenDbProfiler
{
    public class RavenDbProfilerLite : IProfilerLite
    {
        private volatile bool isRunning;

        BlockingCollection<ProfilerRecord> blockingCollection;
        private Task workerTask;

        private static SHA256 sha = SHA256.Create();

        private readonly IDocumentStore store;
        private readonly string database;
        private readonly ILogger<RavenDbProfilerLite> logger;
        private readonly TimeSpan expiration;

        private static Regex listOfValuesRegex = new Regex(@"\((?:(?:[:@]P)*\d+(?:,\s*(?:[:@]P)*\d+)*)\)", RegexOptions.Compiled);

        public RavenDbProfilerLite(IDocumentStore store, string database, TimeSpan expiration, ILogger<RavenDbProfilerLite> logger)
        {
            this.store = store;
            this.database = database;
            this.logger = logger;
            this.expiration = expiration;
        }

        public void StartProfiling()
        {
            if (isRunning) throw new InvalidOperationException("Already running");
            logger.LogInformation("started profiling");
            isRunning = true;
            blockingCollection = new BlockingCollection<ProfilerRecord>(10 * 1024);
            this.workerTask = ProcessQueue();

        }

        public async Task StopProfiling()
        {
            this.StopProfilingImpl();
            await this.workerTask;
            this.workerTask = null;
        }

        private void StopProfilingImpl()
        {
            try
            {
                if (!isRunning) throw new InvalidOperationException("Not running");
                isRunning = false;
                blockingCollection.CompleteAdding();
                try { logger.LogInformation("stoped profiling"); } catch { }
            }
            catch (Exception ex)
            {
                try { logger.LogError(ex, "Failed to stop profiling"); } catch { }
            }
        }

        public void LogCommandExecution(DbCommand command, DataService dataService, TimeSpan executionTime)
        {
            if (!isRunning) return;
            try
            {
                var blockingCollection = this.blockingCollection;
                if (blockingCollection == null || blockingCollection.IsAddingCompleted) return;
                var record = new ProfilerRecord
                {
                    CommandText = command.CommandText,
                    CommandType = command.CommandType,
                    Context = dataService.AppContext,
                    ExecutionDate = DateTime.UtcNow,
                    ExecutionDelay = executionTime,
                    UserId = dataService.CurrentUserId,
                    DataServiceInstanceId = dataService.InstanceId,
                    Parameters = command.Parameters.Cast<DbParameter>().ToDictionary(x => x.ParameterName, x => ParameterValue(x))
                };
                if (blockingCollection.TryAdd(record, TimeSpan.Zero))
                {
                    if (logger.IsEnabled(LogLevel.Trace))
                    {
                        var len = Math.Min(command.CommandText.Length, 30);
                        logger.LogTrace("Command execution added to the log queue: " + command.CommandText.Substring(0, len).Replace("\n", " ").Replace("\r", "").Replace("\t", " ") + " ...");
                    }
                }
                else
                {
                    logger.LogWarning("Command execution discarded. The queue is full");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to add command executions lo the log queue");
            }
        }



        private static object ParameterValue(DbParameter p)
        {
            if (Convert.IsDBNull(p.Value)) return p.Value;
            const int maxSize = 4000;
            var s = p.Value as string;
            if (s != null)
            {
                if (s.Length > maxSize) return "#value too large#";
                return s;
            }
            var b = p.Value as byte[];
            if (b != null)
            {
                if (b.Length > maxSize) return "#value too large#";
                return "0x" + ByteArrayToString(b);
            }
            if (p.Size >= 0 && p.Size <= maxSize) return p.Value;
            return "#value too large#";
        }


        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private static string GetHash(string value)
        {
            return ByteArrayToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        private HashSet<string> sqlStatementIds = new HashSet<string>();
        private HashSet<string> normalizedSqlStatementIds = new HashSet<string>();


        private static TimeSpan interval = TimeSpan.FromSeconds(30);
        private static int timout = 1000;
        private async Task ProcessQueue()
        {
            await Task.Yield();
            bool stopRequested = false;
            bool error = false;
            var watch = new Stopwatch();

            BulkInsertOperation bulkInsert = null;
            IAsyncDocumentSession session = null;


            while (true)
            {
                ProfilerRecord record = null;
                try
                {
                    var taken = blockingCollection.TryTake(out record, timout);
                    if (bulkInsert != null && watch.Elapsed > interval)
                    {
                        try { bulkInsert.Dispose(); } catch { }
                        try { session.Dispose(); } catch { }
                        bulkInsert = null;
                        session = null;
                    }
                    if (taken)
                    {
                        if (bulkInsert == null)
                        {
                            bulkInsert = store.BulkInsert(this.database);
                            session = store.OpenAsyncSession(this.database);
                            session.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;
                            watch.Restart();
                        }
                    }
                    else
                    {
                        if (blockingCollection.IsCompleted)
                        {
                            stopRequested = true;
                        }
                        else continue;
                    }
                }
                catch (InvalidOperationException)
                {
                    stopRequested = true;
                }
                catch (OperationCanceledException)
                {
                    stopRequested = true;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to read record from blocking collection");
                    error = true;
                    try { this.StopProfilingImpl(); } catch { }
                }
                if (stopRequested || error)
                {
                    try { this.blockingCollection.Dispose(); } catch { }
                    this.blockingCollection = null;
                    if (bulkInsert != null) try { bulkInsert.Dispose(); } catch { }
                    if (session != null) try { session.Dispose(); } catch { }
                    return;
                }
                try
                {
                    var sqlStatementId = "SqlStatements/" + GetHash(record.CommandText);
                    var normalizedCommandText = listOfValuesRegex.Replace(record.CommandText, "( #ListOfValues# )");
                    var normalizedSqlStatementId = "NormalizedSqlStatements/" + GetHash(normalizedCommandText);

                    if (normalizedSqlStatementIds.Add(normalizedSqlStatementId))
                    {
                        if (!await session.Advanced.ExistsAsync(normalizedSqlStatementId))
                        {
                            var normalizedSqlStatement = new NormalizedSqlStatement
                            {
                                Id = normalizedSqlStatementId,
                                Sql = normalizedCommandText
                            };
                            await bulkInsert.StoreAsync(normalizedSqlStatement, normalizedSqlStatementId);
                        }
                    }

                    if (sqlStatementIds.Add(sqlStatementId))
                    {
                        if (!await session.Advanced.ExistsAsync(sqlStatementId))
                        {
                            var sqlStatement = new SqlStatement
                            {
                                Id = sqlStatementId,
                                NormalizedSqlStatementId = normalizedSqlStatementId,
                                Sql = record.CommandText
                            };
                            await bulkInsert.StoreAsync(sqlStatement, sqlStatementId);
                        }
                    }

                    var sqlStatementExecution = new SqlStatementExecution
                    {
                        CommandType = record.CommandType,
                        Context = record.Context,
                        ExecutionDate = record.ExecutionDate,
                        ExecutionMilliseconds = Math.Round(record.ExecutionDelay.TotalMilliseconds, 3),
                        Id = "SqlStatementExecutions/" + RT.Comb.Provider.PostgreSql.Create().ToString("N"),
                        NormalizedSqlStatementId = normalizedSqlStatementId,
                        Parameters = record.Parameters,
                        SqlStatementId = sqlStatementId,
                        UserId = record.UserId,
                        DataServiceInstanceId = record.DataServiceInstanceId
                    };
                    var metadata = new Dictionary<string, object>
                    {
                        [Raven.Client.Constants.Documents.Metadata.Expires] = DateTime.UtcNow.Add(this.expiration)
                    };

                    await bulkInsert.StoreAsync(sqlStatementExecution, sqlStatementExecution.Id, new MetadataAsDictionary(metadata));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Falied to store EntityLite profile record to ravendb");
                }
            }
        }
    }
}
