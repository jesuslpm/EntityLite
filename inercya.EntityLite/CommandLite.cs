/*
Copyright 2014 i-nercya intelligent software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using inercya.EntityLite.Templates;
using inercya.EntityLite.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace inercya.EntityLite
{
    public abstract class AbstractCommand
    {

        private static ILogger logger;

        private static ILogger Log
        {
            get
            {
                if (logger == null)
                {
                    logger = ConfigurationLite.LoggerFactory.CreateLogger<AbstractCommand>();
                }
                return logger;
            }
        }

        public readonly DataService DataService;
        public readonly bool DisposeCommand;

        public int CommandTimeout { get; set; }

        protected AbstractCommand(DataService dataService, bool disposeCommand)
        {
            if (dataService == null) throw new ArgumentNullException("dataService");
            this.DataService = dataService;
            this.DisposeCommand = disposeCommand;
            this.CommandTimeout = -1;
        }

        protected abstract DbCommand GetCommand();

        public int ExecuteNonQuery()
        {
            return this.ExecuteCommand( cmd =>
            {
                var returnValue = cmd.ExecuteNonQuery();
                SetOutPutParameters(cmd);
                return returnValue;
            });
        }

        public Task<int> ExecuteNonQueryAsync()
        {
           return this.ExecuteCommandAsync(async (cmd) =>
           {
               var returnValue = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
               SetOutPutParameters(cmd);
               return returnValue;
           });
        }

        public object ExecuteScalar()
        {
            return this.ExecuteCommand(cmd =>
            {
                var returnValue = cmd.ExecuteScalar();
                SetOutPutParameters(cmd);
                return returnValue;
            });
        }

        public Task<object> ExecuteScalarAsync()
        {
            return this.ExecuteCommandAsync(async (cmd) =>
            {
                var returnValue = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                SetOutPutParameters(cmd);
                return returnValue;
            });
        }

        public IDataReader ExecuteReader()
        {
            return this.ExecuteCommand(cmd => cmd.ExecuteReader());
        }

        public Task<DbDataReader> ExecuteReaderAsync()
        {
            return this.ExecuteCommandAsync(cmd => cmd.ExecuteReaderAsync());
        }

        protected abstract void SetOutPutParameters(DbCommand command);

        public T FirstOrDefault<T>() where T : class, new()
        {
            return this.ToEnumerable<T>().FirstOrDefault();
        }

        public async Task<T> FirstOrDefaultAsync<T>() where T : class, new()
        {
            return (await this.ToEnumerableAsync<T>().ConfigureAwait(false)).FirstOrDefault();
        }

        public IEnumerable<T> ToEnumerable<T>() where T : class, new()
        {
            DbCommand command = null;
            Stopwatch watch;
            IDataReader reader = null;
            try
            {
                int maxRetries = DataService.IsActiveTransaction ? 0 : DataService.MaxRetries;
                command = ConfigureCommandAndOpenConnection();
                Func<DbDataReader> func = command.ExecuteReader;
                watch = Stopwatch.StartNew();
                reader = func.ExecuteWithRetries(
                        maxRetries, DataService.InitialMillisecondsRetryDelay,
                        (ex, willRetry) => DataService.NotifyErrorOcurred(ex, willRetry));
            }
            catch (Exception ex)
            {
                if (command == null)
                {
                    Log.LogError(ex, "Couldn't get data reader from command");
                }
                else
                {
                    Log.LogError(ex, string.Format("Couldn't get data reader from command\r\n{0}\r\n{1}", command.CommandText, command.GetParamsAsString()));
                }
                throw;
            }
            return reader.ToEnumerable<T>(() =>
            {
                try
                {
                    SetOutPutParameters(command);
                    LogQueryExecution(command, watch.Elapsed);
                    ConfigurationLite.Profiler.LogCommandExecution(command, DataService, watch.Elapsed);
                }
                finally
                {
                    if (DisposeCommand && command != null) command.Dispose();
                }
            });
        }

        private void LogQueryExecution(DbCommand cmd, TimeSpan timeTaken)
        {
            Log.LogInformation("Query completed in {0}.\r\n{1}\r\nParameters: {2}", timeTaken, cmd.CommandText, cmd.GetParamsAsString());
        }

        private void LogCommandExecution(DbCommand cmd, TimeSpan timeTaken)
        {
            Log.LogInformation("Command completed in {0}.\r\n{1}\r\nParameters: {2}", timeTaken, cmd.CommandText, cmd.GetParamsAsString());
        }

        public async Task<IEnumerable<T>> ToEnumerableAsync<T>() where T : class, new()
        {
            DbCommand command = null;
            Stopwatch watch;
            IDataReader reader = null;
            try
            {
                int maxRetries = DataService.IsActiveTransaction ? 0 : DataService.MaxRetries;
                command = await ConfigureCommandAndOpenConnectionAsync().ConfigureAwait(false);
                Func<Task<DbDataReader>> func = command.ExecuteReaderAsync;
                watch = Stopwatch.StartNew();
                reader = await func.ExecuteWithRetriesAsync(
                        maxRetries, DataService.InitialMillisecondsRetryDelay,
                        (ex, willRetry) => DataService.NotifyErrorOcurred(ex, willRetry))
                        .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (command == null)
                {
                    Log.LogError(ex, "Couldn't get data reader from command");
                }
                else
                {
                    Log.LogError(ex, string.Format("Couldn't get data reader from command\r\n{0}\r\n{1}", command.CommandText, command.GetParamsAsString()));
                }
                throw;
            }
            return reader.ToEnumerable<T>(() =>
            {
                try
                {
                    SetOutPutParameters(command);
                    LogQueryExecution(command, watch.Elapsed);
                    ConfigurationLite.Profiler.LogCommandExecution(command, DataService, watch.Elapsed);
                }
                finally
                {
                    if (DisposeCommand && command != null) command.Dispose();
                }
            });
        }

        public IList<T> ToList<T>() where T : class, new()
        {
            return this.ToEnumerable<T>().ToList();
        }

        public async Task<IList<T>> ToListAsync<T>() where T : class, new()
        {
            DbCommand command = null;
            Stopwatch watch;
            DbDataReader reader = null;
            try
            {
                int maxRetries = DataService.IsActiveTransaction ? 0 : DataService.MaxRetries;
                command = await ConfigureCommandAndOpenConnectionAsync().ConfigureAwait(false);
                Func<Task<DbDataReader>> func = command.ExecuteReaderAsync;
                watch = Stopwatch.StartNew();
                reader = await func.ExecuteWithRetriesAsync(
                        maxRetries, DataService.InitialMillisecondsRetryDelay,
                        (ex, willRetry) => DataService.NotifyErrorOcurred(ex, willRetry))
                        .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (command == null)
                {
                    Log.LogError(ex, "Couldn't get data reader from command");
                }
                else
                {
                    Log.LogError(ex, string.Format("Couldn't get data reader from command\r\n{0}\r\n{1}", command.CommandText, command.GetParamsAsString()));
                }
                throw;
            }
            return await reader.ToListAsync<T>(() =>
            {
                try
                {
                    SetOutPutParameters(command);
                    LogQueryExecution(command, watch.Elapsed);
                    ConfigurationLite.Profiler.LogCommandExecution(command, DataService, watch.Elapsed);
                }
                finally
                {
                    if (DisposeCommand && command != null) command.Dispose();
                }
            }).ConfigureAwait(false);
        }


        private DbCommand ConfigureCommand()
        {
            var command = GetCommand();
            command.Connection = this.DataService.Connection;
            if (this.CommandTimeout >= 0) command.CommandTimeout = this.CommandTimeout;
            else if (this.DataService.CommandTimeout >= 0) command.CommandTimeout = this.DataService.CommandTimeout;
            if (DataService.IsActiveTransaction)
            {
                command.Transaction = DataService.Transaction;
            }
            return command;
        }

        private DbCommand ConfigureCommandAndOpenConnection()
        {
            var cmd = ConfigureCommand();
            this.DataService.OpenConnection();
            return cmd;
        }

        private async Task<DbCommand> ConfigureCommandAndOpenConnectionAsync()
        {
            var cmd = ConfigureCommand();
            await this.DataService.OpenConnectionAsync().ConfigureAwait(false);
            return cmd;
        }

        private async Task<T> ExecuteCommandAsync<T>(Func<DbCommand, Task<T>> executeCommandAsyncFunc)
        {
            DbCommand command = null;
            try
            {
                int maxRetries = DataService.IsActiveTransaction ? 0 : DataService.MaxRetries;
                command = await ConfigureCommandAndOpenConnectionAsync().ConfigureAwait(false);
                Func<Task<T>> func = () => executeCommandAsyncFunc(command);
                var watch = Stopwatch.StartNew();
                var result = await func.ExecuteWithRetriesAsync(
                        maxRetries, DataService.InitialMillisecondsRetryDelay,
                        (ex, willRetry) => DataService.NotifyErrorOcurred(ex, willRetry))
                        .ConfigureAwait(false);
                LogCommandExecution(command, watch.Elapsed);
                ConfigurationLite.Profiler.LogCommandExecution(command, DataService, watch.Elapsed);
                return result;
            }
            catch (Exception ex)
            {
                if (command == null)
                {
                    Log.LogError(ex, "Couldn't execute command");
                }
                else
                {
                    Log.LogError(ex, string.Format("Couldn't execute command\r\n{0}\r\n{1}", command.CommandText, command.GetParamsAsString()));
                }
                throw;
            }
            finally
            {
                if (command != null && DisposeCommand) command.Dispose();
            }
        }

        private T ExecuteCommand<T>(Func<DbCommand, T> executeCommandFunc)
        {
            DbCommand command = null;
            try
            {
                int maxRetries = DataService.IsActiveTransaction ? 0 : DataService.MaxRetries;
                command = ConfigureCommandAndOpenConnection();
                Func<T> func = () =>
                {
                    return executeCommandFunc(command);
                };
                var watch = Stopwatch.StartNew();
                var result = func.ExecuteWithRetries(
                        maxRetries, DataService.InitialMillisecondsRetryDelay,
                        (ex, willRetry) => DataService.NotifyErrorOcurred(ex, willRetry));
                LogCommandExecution(command, watch.Elapsed);
                ConfigurationLite.Profiler.LogCommandExecution(command, DataService, watch.Elapsed);
                return result;
            }
            catch (Exception ex)
            {
                if (command == null)
                {
                    Log.LogError(ex, "Couldn't execute command");
                }
                else
                {
                    Log.LogError(ex, string.Format("Couldn't execute command\r\n{0}\r\n{1}", command.CommandText, command.GetParamsAsString()));
                }
                throw;
            }
            finally
            {
                if (command != null && DisposeCommand) command.Dispose();
            }
        }
    }

    public class CommandExecutor : AbstractCommand
    {

        public CommandExecutor(DataService dataService, bool disposeCommand) : base(dataService, disposeCommand)
        {
        }

        public Func<DbCommand> GetCommandFunc { get; set; }

        public Action<DbCommand> SetOutPutParametersAction { get; set;  }

        protected override DbCommand GetCommand()
        {
            if (GetCommandFunc == null) return null;
            else return GetCommandFunc();
        }


        protected override void SetOutPutParameters(DbCommand command)
        {
            if (SetOutPutParametersAction != null) SetOutPutParametersAction(command);
        }
    }

    public class StoredProcedureExecutor : AbstractCommand
    {
        public StoredProcedureExecutor(DataService dataService, bool disposeCommand): base(dataService, disposeCommand)
        {
        }

        public Func<DbCommand> GetCommandFunc { get; set; }

        protected override DbCommand GetCommand()
        {
            if (GetCommandFunc == null) return null;
            else return GetCommandFunc();
        }

        public readonly Dictionary<string, object> OutputParameterValues = new Dictionary<string, object>();

        protected override void SetOutPutParameters(DbCommand command)
        {
            OutputParameterValues.Clear();
            foreach (var p in command.Parameters.Cast<DbParameter>().Where( x => (x.Direction & ParameterDirection.Output) == ParameterDirection.Output && x.Direction != ParameterDirection.ReturnValue))
            {
                OutputParameterValues.Add(p.SourceColumn, p.Value == DBNull.Value ? null : p.Value);
            }
        }
    }
}
