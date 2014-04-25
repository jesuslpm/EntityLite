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

namespace inercya.EntityLite
{
    public abstract class AbstractCommand
    {

        public static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public readonly DataService DataService;
        public readonly bool DisposeCommand;

        protected AbstractCommand(DataService dataService, bool disposeCommand)
        {
            if (dataService == null) throw new ArgumentNullException("dataService");
            this.DataService = dataService;
            this.DisposeCommand = disposeCommand;
        }

        protected abstract DbCommand GetCommand();
       

        public int ExecuteNonQuery()
        {
            return this.ExecuteCommand(getCommand =>
            {
                DbCommand cmd = null;
                try
                {
                    cmd = getCommand();
                    var returnValue = cmd.ExecuteNonQuery();
                    SetOutPutParameters(cmd);
                    return returnValue;
                }
                finally
                {
                    if (DisposeCommand && cmd != null) cmd.Dispose();
                }
            });
        }

        public object ExecuteScalar()
        {
            return this.ExecuteCommand(getCommand =>
            {
                DbCommand cmd = null;
                try
                {
                    cmd = getCommand();
                    var returnValue = cmd.ExecuteScalar();
                    SetOutPutParameters(cmd);
                    return returnValue;
                }
                finally
                {
                    if (DisposeCommand && cmd != null) cmd.Dispose();
                }
            });
        }

        public IDataReader ExecuteReader()
        {
            return this.ExecuteCommand(getCommand =>
            {
                DbCommand cmd = null;
                try
                {
                    cmd = getCommand();
                    return cmd.ExecuteReader();
                }
                finally
                {
                    if (DisposeCommand && cmd != null) cmd.Dispose();
                }
            });
        }

        protected abstract void SetOutPutParameters(DbCommand command);

        public T FirstOrDefault<T>() where T : class, new()
        {
            return this.ToEnumerable<T>().FirstOrDefault();
        }

        public IEnumerable<T> ToEnumerable<T>() where T : class, new()
        {
            DbCommand command = null;
            try
            {
                int maxRetries = DataService.IsActiveTransaction ? 0 : DataService.MaxRetries;
                
                Func<IDataReader> func = () =>
                {
                    command = GetConfiguredCommand();
                    return command.ExecuteReader();
                };
                var watch = Stopwatch.StartNew();
                var reader = func.ExecuteWithRetries(
                        maxRetries, DataService.InitialMillisecondsRetryDelay,
                        (ex, willRetry) => DataService.NotifyErrorOcurred(ex, willRetry));
                using (reader)
                {
                    var factory = reader.GetFactory(typeof(T));
                    while (reader.Read())
                    {
                        yield return (T)factory(reader);
                    }
                }
                SetOutPutParameters(command);
                ProfilerLite.LogCommandExecution(command, DataService, watch.ElapsedMilliseconds);
            }
            finally
            {
                if (DisposeCommand && command != null) command.Dispose();
            }
        }

        public IList<T> ToList<T>() where T : class, new()
        {
            return this.ToEnumerable<T>().ToList();
        }


        private DbCommand GetConfiguredCommand()
        {
            this.DataService.OpenConnection();
            var command = GetCommand();
            command.Connection = DataService.Connection;
            if (DataService.IsActiveTransaction)
            {
                command.Transaction = DataService.Transaction;
            }
            return command;
        }

        private T ExecuteCommand<T>(Func<Func<DbCommand>, T> executeCommandFunc)
        {
            try
            {
                int maxRetries = DataService.IsActiveTransaction ? 0 : DataService.MaxRetries;
                
                DbCommand command = null;
                Func<T> func = () =>
                {
                    return executeCommandFunc(() => command = GetConfiguredCommand());
                };
                var watch = Stopwatch.StartNew();
                var result = func.ExecuteWithRetries(
                        maxRetries, DataService.InitialMillisecondsRetryDelay,
                        (ex, willRetry) => DataService.NotifyErrorOcurred(ex, willRetry));
                ProfilerLite.LogCommandExecution(command, DataService, watch.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                Log.ErrorException("Couldn't execute command", ex);
                throw;
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
