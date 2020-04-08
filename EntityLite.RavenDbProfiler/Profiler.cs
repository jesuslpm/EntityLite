using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
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

        public RavenDbProfilerLite(IDocumentStore store, string database, TimeSpan expiration,  ILogger<RavenDbProfilerLite> logger)
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
            blockingCollection = new BlockingCollection<ProfilerRecord>();
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
                logger.LogInformation("stoped profiling");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to stop profiling");
            }
        }


        public void LogCommandExecution(DbCommand command, DataService dataService, TimeSpan executionTime)
        {
            if (isRunning)
            {
                var blockingCollection = this.blockingCollection;
                if (blockingCollection == null || blockingCollection.IsAddingCompleted) return;
                var record = new ProfilerRecord
                {
                    CommandText = command.CommandText,
                    CommandType = command.CommandType,
                    Context = dataService?.ApplicationContextGetter(),
                    ExecutionDate = DateTime.UtcNow,
                    ExecutionDelay = executionTime,
                    UserId = dataService.CurrentUserId,
                    Parameters = command.Parameters.Cast<DbParameter>().ToDictionary(x => x.ParameterName, x => x.Size < 0 || x.Size > 4000 ? "#value too large#" : x.Value)
                };
                try
                {
                    blockingCollection.Add(record);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to add record to blocking collection");
                }
            }
            
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

        private async Task ProcessQueue()
        {
            bool stopRequested = false;
            bool error = false;
            using (var bulkInsert = new  BulkInsertOperation(this.database, this.store))
            using (var session = this.store.OpenAsyncSession(this.database))
            {
                session.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;
                while (true) 
                {
                    ProfilerRecord record = null;
                    try
                    {
                        record = blockingCollection.Take();
                    }
                    catch (ObjectDisposedException)
                    {
                        stopRequested = true;
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
                        return;
                    }

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
                            await bulkInsert.StoreAsync(normalizedSqlStatementId, normalizedSqlStatementId);
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

                    };
                    var metadata = new Dictionary<string, object>
                    {
                        [Constants.Documents.Metadata.Expires] = DateTime.UtcNow.Add(this.expiration)
                    };
                    await bulkInsert.StoreAsync(sqlStatementExecution, sqlStatementExecution.Id, new MetadataAsDictionary(metadata));
                }
            }
        }
    }
}
