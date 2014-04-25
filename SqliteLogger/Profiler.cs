using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using inercya.EntityLite.SqliteProfiler.Entities;

namespace inercya.EntityLite.SqliteProfiler
{
    public class LogItem
    {
        public string CommandText;
        public long ExecutionTime;
        public string Params;
    }


    public class Profiler : inercya.EntityLite.IProfilerLite
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        SafeQueue<LogItem> logItems;
        private AutoResetEvent signal;
        private volatile bool IsRunning;
        private object syncObject = new object();
        private Thread workingThread;

        public Profiler()
        {
            logItems = new SafeQueue<LogItem>();
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


        public void LogCommandExecution(DbCommand command, DataService dataService, long executionTimeInMilliseconds)
        {
            if (!IsRunning || dataService is SqliteLoggerDataService) return;
            var item = new LogItem
            {
                CommandText = command.CommandText,
                ExecutionTime = executionTimeInMilliseconds,
                Params = GetParamsAsString(command)
            };
            logItems.Enqueue(item);
            signal.Set();
        }

        public void StopProfiling()
        {
            if (!IsRunning) throw new InvalidOperationException("Already stopped");
            lock (syncObject)
            {
                if (!IsRunning) throw new InvalidOperationException("Already stopped");
                IsRunning = false;
                signal.Set();
                workingThread.Join();
                workingThread = null;
            }
        }


        private void ProcessLogItems()
        {
            SqliteLoggerDataService dataService = null;
            try
            {
                while (true)
                {
                    signal.WaitOne();
                    if (!IsRunning)
                    {
                        if (dataService != null)
                        {
                            dataService.Dispose();
                            return;
                        }
                    }
                    if (dataService == null)
                    {
                        dataService = SqliteLoggerDataService.Create();
                    }
                    else if (dataService.FileName != SqliteLoggerDataService.GetFileName())
                    {
                        try
                        {
                            dataService.Dispose();
                        }
                        catch (Exception ex)
                        {
                            log.ErrorException("Error disposing data service", ex);
                        }
                        dataService = SqliteLoggerDataService.Create();
                    }
                    LogItem item = null;
                    try
                    {
                        dataService.BeginTransaction();
                        int itemCount = 0;
                        while (logItems.Dequeue(out item))
                        {
                            dataService.LogCommandExecution(item, false);
                            itemCount++;
                        }
                        dataService.Commit();
                        Debug.WriteLine(itemCount + " items in queue");
                    }
                    catch (Exception ex)
                    {
                        log.ErrorException("Error Profiling", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("Error Profiling", ex);
            }
            finally
            {
                this.IsRunning = false;
            }
        }

        string GetParamsAsString(DbCommand command)
        {
            StringBuilder builder = new StringBuilder();
            bool firstTime = true;

            foreach (DbParameter p in command.Parameters)
            {
                if (p.Value != null)
                {
                    if (firstTime)
                    {
                        firstTime = false;
                    }
                    else
                    {
                        builder.Append(", ");
                    }
                    string paramValueAsString = null;
                    if (Convert.IsDBNull(p.Value))
                    {
                        paramValueAsString = "NULL";
                    }
                    else
                    {
                        IConvertible convertible = p.Value as IConvertible;
                        if (convertible != null)
                        {
                            paramValueAsString = convertible.ToString(CultureInfo.InvariantCulture);
                        }
                        else if (p.Value is byte[])
                        {
                            paramValueAsString = "0x" + BitConverter.ToString((byte[])p.Value, 0).Replace("-", string.Empty);
                        }
                        else
                        {
                            paramValueAsString = p.Value.ToString();
                        }
                    }
                    builder.Append(p.ParameterName).Append(" = ").Append(paramValueAsString);
                }
            }
            return builder.ToString();
        }
    }
}
