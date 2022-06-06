using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using inercya.EntityLite.SqliteProfiler.Entities;
using System.IO;
using inercya.EntityLite.Collections;
using inercya.EntityLite.Extensions;
using Microsoft.Extensions.Logging;

namespace inercya.EntityLite.SqliteProfiler
{
    public class LogItem
    {
        public string CommandText { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string Params { get; set; }
        public string ApplicationContext { get; set; }
        public Guid DataServiceInstanceId { get; set; }
    }

    public enum ProfileFileFrecuency
    {
        Daily,
        Weekly,
        Monthly
    }


    public sealed class Profiler : inercya.EntityLite.IProfilerLite, IDisposable
    {
        private static ILogger logger;

        private static bool isLoggerInitialized;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "logging should not throw")]
        private static ILogger Log
        {
            get
            {
                if (!isLoggerInitialized)
                {
                    isLoggerInitialized = true;
                    try
                    {
                        logger = ConfigurationLite.LoggerFactory.CreateLogger<Profiler>();
                    }
                    catch { }
                }
                return logger;
            }
        }

        Queue<LogItem> logItemsQueue;
        private AutoResetEvent signal;
        private volatile bool _isRunning;
        private object syncObject = new object();
        private Thread workingThread;

        public ProfileFileFrecuency ProfileDatabaseFrecuency { get; private set; }
        public string ProfileDatabaseFileNamePrefix { get; private set; }
        public int MaxProfileDatabaseFiles { get; private set; }
        public string ProfileDatabaseFolderPath { get; private set; }
        public bool FullLogging { get; private set; }

        private DateTime lastDataServiceCreateDay;

        public bool IsRunning
        {
            get { return _isRunning; }
            private set { _isRunning = value; }
        }


        public Profiler(
            string profileDatabaseFolderPath,
            ProfileFileFrecuency profileFileFrecuency = ProfileFileFrecuency.Weekly,
            bool fullLogging = false,
            int maxProfileDatabaseFiles = 6,
            string profileDatabaseFileNamePrefix = "EntityLiteProfile-")
        {
            this.ProfileDatabaseFolderPath = profileDatabaseFolderPath;
            this.ProfileDatabaseFrecuency = profileFileFrecuency;
            this.MaxProfileDatabaseFiles = maxProfileDatabaseFiles;
            this.ProfileDatabaseFileNamePrefix = profileDatabaseFileNamePrefix;
            this.FullLogging = fullLogging;
 
            logItemsQueue = new Queue<LogItem>();
            signal = new AutoResetEvent(false);
        }

        public void StartProfiling()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Already profiling");
            }
            lock (syncObject)
            {
                if (IsRunning) throw new InvalidOperationException("Already profiling");
                IsRunning = true;
                workingThread = new Thread(ProcessLogItems);
                workingThread.Start();
            }
        }

        private string GetDatabaseFilePath()
        {
            DateTime today = DateTime.Today;
            switch (this.ProfileDatabaseFrecuency)
            {
                case ProfileFileFrecuency.Daily:
                    return Path.Combine(this.ProfileDatabaseFolderPath, string.Format(CultureInfo.InvariantCulture, "{0}{1}.db", this.ProfileDatabaseFileNamePrefix, today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
                case ProfileFileFrecuency.Weekly:
                    return Path.Combine(this.ProfileDatabaseFolderPath, string.Format(CultureInfo.InvariantCulture, "{0}{1:0000}-W{2:00}.db", this.ProfileDatabaseFileNamePrefix, today.Year,
                        CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstDay, DayOfWeek.Monday)));
                case ProfileFileFrecuency.Monthly:
                    return Path.Combine(this.ProfileDatabaseFolderPath, string.Format(CultureInfo.InvariantCulture, "{0}{1}.db", this.ProfileDatabaseFileNamePrefix, today.ToString("yyyy-'M'MM", CultureInfo.InvariantCulture)));
                default:
                    throw new InvalidOperationException("Invalid ProfileDatabaseFrecuency");
            }
        }

        const int MaxAllowedQueueLength = 8192;


        public void LogCommandExecution(DbCommand command, DataService dataService, TimeSpan executionTime)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (dataService == null) throw new ArgumentNullException(nameof(dataService));
            if (!IsRunning || dataService is SqliteProfilerDataService) return;
            if (logItemsQueue.Count >= MaxAllowedQueueLength) return;
            var item = new LogItem
            {
                CommandText = command.CommandText,
                ExecutionTime = executionTime,
                Params = command.GetParamsAsString(),
                ApplicationContext = dataService.ApplicationContextGetter?.Invoke(),
                DataServiceInstanceId = dataService.InstanceId
            };
            lock (logItemsQueue)
            {
                var logItemCount = logItemsQueue.Count;
                if (logItemCount >= MaxAllowedQueueLength) return;
                if (logItemCount >= MaxQueueLength) MaxQueueLength = logItemCount + 1;
                logItemsQueue.Enqueue(item);
            }
            signal.Set();
        }

        public void StopProfiling()
        {
            if (!IsRunning) return; ;
            lock (syncObject)
            {
                if (!IsRunning) return;
                IsRunning = false;
                signal.Set();
                workingThread.Join();
                workingThread = null;
            }
        }

        public int MaxQueueLength { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Dataservice is ensured to be disposed")]
        private void ProcessLogItems()
        {
            SqliteProfilerDataService dataService = null;
            var startTime = DateTime.UtcNow;
            int attempt = 1;
            try {
                while (IsRunning)
                {
                    signal.WaitOne();
                   
                    try
                    {
                        dataService = EnsureDataServiceAndDeleteOldFiles(dataService);
                        // Log?.LogTrace("DataService Ensured");
                        startTime = DateTime.UtcNow;
                        attempt = 1;
                    }
#pragma warning disable CA1031 // I don't care which exception is thrown, it it fails I need to retry.
                    catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                    {
                        if (++attempt > 10 && DateTime.UtcNow.Subtract(startTime) > TimeSpan.FromMinutes(2))
                        {
                            Log?.LogError(ex, "Error opening SqliteProfilerDataService");
                            return;
                        }
                        continue;
                    }
                    var processLogItemsStartTime = DateTime.UtcNow;
                    try
                    {
                        ProcessLogItems(dataService);
                    }
#pragma warning disable CA1031 // I don't care which exception is thrown. If it fails I need to log it and continue
                    catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                    {
                        Log?.LogError(ex, "Error processing log items after {Elapsed}", DateTime.UtcNow.Subtract(processLogItemsStartTime));
                        signal.Set();
                    }
                } 
            }
            finally
            {
                IsRunning = false;
                TryDisposeDataService(dataService);
                Log?.LogInformation("SqliteProfiler stoped");
            }
        }


        private List<LogItem> logItemBuffer = new List<LogItem>(128);

        private bool TryDequeue()
        {
            logItemBuffer.Clear();
            lock (logItemsQueue)
            {
                while (logItemsQueue.Count > 0 && logItemBuffer.Count < 128)
                {
                    var item = logItemsQueue.Dequeue();
                    logItemBuffer.Add(item);
                }
            }
            return logItemBuffer.Count > 0;
        }

        private void ProcessLogItems(SqliteProfilerDataService dataService)
        {
            if (logItemsQueue.Count == 0) return;
            var watch = Stopwatch.StartNew();
            dataService.BeginTransaction();
            try
            {
                int itemCount = 0;
                while (TryDequeue())
                {
                    foreach (var item in logItemBuffer)
                    {
                        dataService.LogCommandExecution(item, this.FullLogging);
                        itemCount++;
                    }
                }
                dataService.Commit();
                Log?.LogTrace("Logged {Count} command executions in {Elapsed}", itemCount, watch.Elapsed);
            }
            catch
            {
                try { dataService.Rollback(); } catch { }
                throw;
            }
            watch.Stop();
        }

        private static void TryDisposeDataService(SqliteProfilerDataService dataService)
        {
            if (dataService != null)
            {
                try
                {
                    dataService.Dispose();
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, "Error disposing data service");
                }
            }
        }

        public int MaxDataServiceAttempts { get; private set; }
        private SqliteProfilerDataService EnsureDataServiceAndDeleteOldFiles(SqliteProfilerDataService dataService)
        {
            string filePath = null;
            if (dataService == null || (DateTime.Today != this.lastDataServiceCreateDay && dataService.FilePath != (filePath = this.GetDatabaseFilePath())))
            {
                TryDisposeDataService(dataService);
                if (filePath == null) filePath = this.GetDatabaseFilePath();
                int attempt = 1;
                while (true)
                {
                    try
                    {
                        dataService = SqliteProfilerDataService.Create(filePath);
                        lastDataServiceCreateDay = DateTime.Today;
                        dataService.OpenConnection();
                        ThreadPool.QueueUserWorkItem(DeleteOldFiles);
                        return dataService;
                    }
                    catch
                    {
                        TryDisposeDataService(dataService);
                        if (++attempt > 20) throw;
                        Thread.Sleep(200);
                    }
                    finally
                    {
                        if (attempt > MaxDataServiceAttempts) MaxDataServiceAttempts = attempt;
                    }
                }
            }
            return dataService;
        }

        private void DeleteOldFiles(object state)
        {
            try
            {
                var fileInfos = new List<FileInfo>();
                var files = Directory.GetFiles(this.ProfileDatabaseFolderPath, this.ProfileDatabaseFileNamePrefix + "*.db");
                if (files.Length <= this.MaxProfileDatabaseFiles) return;
                foreach (var filePah in files)
                {
                    fileInfos.Add(new FileInfo(filePah));
                }
                fileInfos.Sort((fi1, fi2) => fi1.LastWriteTime.CompareTo(fi2.LastWriteTime));
                for (int i = 0; i < fileInfos.Count - this.MaxProfileDatabaseFiles; i++)
                {
                    fileInfos[i].Delete();
                }
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, "Error deleting old profiler database files");
            }
        }

        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            var disposable = this.signal as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
