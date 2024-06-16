﻿/*
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.Common;
using inercya.EntityLite.Extensions;
using System.Diagnostics;
using inercya.EntityLite.Builders;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace inercya.EntityLite
{
    public abstract class AbstractQueryLite : IQueryLite
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
                        logger = ConfigurationLite.LoggerFactory?.CreateLogger<AbstractQueryLite>();
                    }
                    catch { }
                }
                return logger;
            }
        }
        public IList<string> FieldList { get; set; }
        public ICollection<ConditionLite> Filter { get; set; }
        public ICollection<SortDescriptor> Sort { get; set; }
		public ICollection<string> Options { get; set; }
        public Alias Alias { get; set; }

        [NonSerialized]
        private DataService _dataService;
        public DataService DataService { get { return _dataService; } set { _dataService = value; } }

        [NonSerialized]
        private IQueryBuilder _queryBuilder;
        public IQueryBuilder QueryBuilder { get { return _queryBuilder; } set { _queryBuilder = value; } }
        public int CommandTimeout { get; set; }
		protected abstract IEnumerable NonGenericToEnumerable();
        IEnumerable IQueryLite.ToEnumerable()
		{
			return NonGenericToEnumerable();
		}
		protected abstract IEnumerable NonGenericToEnumerable(int fromIndex, int toIndex);
        IEnumerable IQueryLite.ToEnumerable(int fromIndex, int toIndex)
		{
			return NonGenericToEnumerable(fromIndex, toIndex);
		}
        IList IQueryLite.ToList()
        {
            return NonGenericToList();
        }
        protected abstract IList NonGenericToList();
        IList IQueryLite.ToList(int fromIndex, int toIndex)
        {
            return NonGenericToList(fromIndex, toIndex);
        }
        protected abstract IList NonGenericToList(int fromIndex, int toIndex);
        protected abstract object NonGenericFirstOrDefault();
        object IQueryLite.FirstOrDefault()
		{
			return NonGenericFirstOrDefault();
		}
        [NonSerialized]
        private Type _entityType;
        public virtual Type EntityType { get { return _entityType; } set { _entityType = value; } }

        protected internal DbCommand GetSelectCommand()
        {
            DbCommand selectCommand = this.DataService.EntityLiteProvider.CreateCommand();
            int paramIndex = 0;
            selectCommand.CommandText = QueryBuilder.GetSelectQuery(selectCommand, ref paramIndex, 0);
            return selectCommand;
        }

        protected internal DbCommand GetDeleteCommand()
        {
            DbCommand cmd = this.DataService.EntityLiteProvider.CreateCommand();
            int paramIndex = 0;
            cmd.CommandText = QueryBuilder.GetDeleteQuery(cmd, ref paramIndex, 0);
            return cmd;
        }

        protected internal DbCommand GetSelectIntoCommand(string destinationTableName)
        {
            DbCommand cmd = this.DataService.EntityLiteProvider.CreateCommand();
            int paramIndex = 0;
            cmd.CommandText = QueryBuilder.GetSelectIntoQuery(cmd, ref paramIndex, 0, destinationTableName);
            return cmd;
        }

        protected internal DbCommand GetInsertIntoCommand(string destinationTableName, string[] columnNames)
        {
            DbCommand cmd = this.DataService.EntityLiteProvider.CreateCommand();
            int paramIndex = 0;
            cmd.CommandText = QueryBuilder.GetInsertIntoQuery(cmd, ref paramIndex, 0, destinationTableName, columnNames);
            return cmd;
        }

        protected DbCommand GetSelectCommand(int fromIndex, int toIndex)
        {
            DbCommand selectCommand = this.DataService.EntityLiteProvider.CreateCommand();
            int paramIndex = 0;
            selectCommand.CommandText = QueryBuilder.GetSelectQuery(selectCommand, ref paramIndex, fromIndex, toIndex, 0);
            return selectCommand;
        }

        protected DbCommand GetCountCommand()
        {
            DbCommand selectCommand = this.DataService.EntityLiteProvider.CreateCommand();
            int paramIndex = 0;
            selectCommand.CommandText =QueryBuilder.GetCountQuery(selectCommand, ref paramIndex);
            return selectCommand;
        }

        protected DbCommand GetAnyCommand()
        {
            DbCommand selectCommand = this.DataService.EntityLiteProvider.CreateCommand();
            int paramIndex = 0;
            selectCommand.CommandText = QueryBuilder.GetAnyQuery(selectCommand, ref paramIndex);
            return selectCommand;
        }

        public int GetCount()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetCountCommand,
                CommandTimeout = this.CommandTimeout
            };
            return Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture);
        }

        public int Delete()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetDeleteCommand,
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ExecuteNonQuery();
        }

        public int SelectInto(string destinationTableName)
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = () => this.GetSelectIntoCommand(destinationTableName),
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ExecuteNonQuery();
        }

        public int InsertInto(string destinationTableName, params string[] columnNames)
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = () => this.GetInsertIntoCommand(destinationTableName, columnNames),
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ExecuteNonQuery();
        }

        public int InsertInto<T>(string destinationTableName, params string[] propertyNames)
        {
            var columnNames = this.QueryBuilder.GetColumns(typeof(T), propertyNames);
            return InsertInto(destinationTableName, columnNames);
        }

        private int InsertIntoBaseTable(Type entityType, string[] propertyNames)
        {
            var metadata = this.EntityType.GetEntityMetadata();
            var destinationTableName = metadata.GetFullTableName(this.DataService.EntityLiteProvider.DefaultSchema, this.DataService.EntityLiteProvider.StartQuote, this.DataService.EntityLiteProvider.EndQuote);
            var columnNames = this.QueryBuilder.GetColumns(entityType, propertyNames).ToArray();
            return InsertInto(destinationTableName, columnNames);
        }

        public int InsertIntoBaseTable(params string[] propertyNames)
        {
            return InsertIntoBaseTable(this.EntityType, propertyNames);
        }

        public int InsertIntoBaseTable<T>(params string[] propertyNames)
        {
            return InsertIntoBaseTable(typeof(T), propertyNames);
        }

        public bool Any()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetAnyCommand,
                CommandTimeout = this.CommandTimeout
            };

            return Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture) == 1; 
        }


        protected AbstractQueryLite()
        {
            this.Filter = new List<ConditionLite>();
            this.Sort = new List<SortDescriptor>();
			this.Options = new List<string>();
            this.FieldList = new List<string>();
            this.CommandTimeout = -1;
        }

        public DataTable Pivot(params PivotTransform[] pivotTransforms)
        {
            return Pivot(PivotExtensions.DefaultPivotedColumnComparison, pivotTransforms);
        }

        public DataTable Pivot(Comparison<PivotedColumn> pivotedColumnComparison, params PivotTransform[] pivotTransforms)
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetSelectCommand,
                CommandTimeout = this.CommandTimeout
            };

            var properties = this.EntityType.GetEntityMetadata().Properties;

            IEnumerable<string> unpivotedPropertyNames = this.Sort
                                .Where(x => !pivotTransforms.Any(p => p.PivotColumnName == x.FieldName || p.ValueColumnName == x.FieldName))
                                .Select(x => x.FieldName);

            using (var reader = cmd.ExecuteReader())
            {
                PivotDef pivotDef = new PivotDef
                {
                    UnpivotedColumnNames = unpivotedPropertyNames.ToArray(),
                    PivotTransforms = pivotTransforms
                };
                return reader.Pivot(pivotDef, pivotedColumnComparison);
            }
        }

        protected abstract Task<IEnumerable> NonGenericToEnumerableAsync();

        Task<IEnumerable> IQueryLite.ToEnumerableAsync()
        {
            return NonGenericToEnumerableAsync();
        }

        protected abstract Task<IEnumerable> NonGenericToEnumerableAsync(int fromIndex, int toIndex);

        Task<IEnumerable> IQueryLite.ToEnumerableAsync(int fromIndex, int toIndex)
        {
            return NonGenericToEnumerableAsync(fromIndex, toIndex);
        }

        Task<IList> IQueryLite.ToListAsync()
        {
            return NonGenericToListAsync();
        }

        protected abstract Task<IList> NonGenericToListAsync();

        Task<IList> IQueryLite.ToListAsync(int fromIndex, int toIndex)
        {
            return NonGenericToListAsync(fromIndex, toIndex);
        }

        protected abstract Task<IList> NonGenericToListAsync(int fromIndex, int toIndex);

        protected abstract Task<object> NonGenericFirstOrDefaultAsync();

        Task<object> IQueryLite.FirstOrDefaultAsync()
        {
            return NonGenericFirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetCountCommand,
                CommandTimeout = this.CommandTimeout
            };
            object result = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
            return Convert.ToInt32(result, CultureInfo.InvariantCulture);
        }

        public Task<int> DeleteAsync()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetDeleteCommand,
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ExecuteNonQueryAsync();
        }

        public Task<int> SelectIntoAsync(string destinationTableName)
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = () => this.GetSelectIntoCommand(destinationTableName),
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> AnyAsync()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetAnyCommand,
                CommandTimeout = this.CommandTimeout
            };
            var result = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
            return Convert.ToInt32(result, CultureInfo.InvariantCulture) == 1;
        }

        public Task<int> InsertIntoAsync(string destinationTableName, params string[] columnNames)
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = () => this.GetInsertIntoCommand(destinationTableName, columnNames),
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ExecuteNonQueryAsync();
        }

        public Task<int> InsertIntoAsync<T>(string destinationTableName, params string[] propertyNames)
        {
            var columnNames = this.QueryBuilder.GetColumns(typeof(T), propertyNames).ToArray();
            return InsertIntoAsync(destinationTableName, columnNames);
        }

        private Task<int> InsertIntoBaseTableAsync(Type entityType, string[] propertyNames)
        {
            var metadata = entityType.GetEntityMetadata();
            var destinationTableName = metadata.GetFullTableName(this.DataService.EntityLiteProvider.DefaultSchema, this.DataService.EntityLiteProvider.StartQuote, this.DataService.EntityLiteProvider.EndQuote);
            var columnNames = this.QueryBuilder.GetColumns(entityType, propertyNames).ToArray();
            return InsertIntoAsync(destinationTableName, columnNames);
        }

        public Task<int> InsertIntoBaseTableAsync(params string[] propertyNames)
        {
            return InsertIntoBaseTableAsync(this.EntityType, propertyNames);
        }

        public Task<int> InsertIntoBaseTableAsync<T>(params string[] propertyNames)
        {
            return InsertIntoBaseTableAsync(typeof(T), propertyNames);
        }
    }

    public class AbstractQueryLite<TEntity> : AbstractQueryLite, IQueryLite<TEntity> where TEntity : class
    {

        private static ILogger logger;

        private static ILogger Log
        {
            get
            {
                if (logger == null)
                {
                    logger = ConfigurationLite.LoggerFactory?.CreateLogger("inercya.EntityLite.QueryLite<" + typeof(TEntity).Name + ">");
                }
                return logger;
            }
        }



        protected AbstractQueryLite() : base()
        {
            this.EntityType = typeof(TEntity);
        }


		public virtual IEnumerable<TEntity> ToEnumerable()
		{
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetSelectCommand,
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ToEnumerable<TEntity>();
		}

        public virtual IEnumerable<TEntity> ToEnumerable(int fromIndex, int toIndex)
		{
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = () => GetSelectCommand(fromIndex, toIndex),
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ToEnumerable<TEntity>();
        }

        public TEntity FirstOrDefault()
		{
			return this.ToEnumerable().FirstOrDefault();
		}

        protected override object NonGenericFirstOrDefault()
		{
			return this.FirstOrDefault();
		}

        public virtual List<TEntity> ToList()
		{
			return ToEnumerable().ToList();
		}

        public virtual List<TEntity> ToList(int fromIndex, int toIndex)
        {
			return ToEnumerable(fromIndex, toIndex).ToList();
        }

        protected override IEnumerable NonGenericToEnumerable()
		{
			return ToEnumerable();
		}

        protected override IEnumerable NonGenericToEnumerable(int fromIndex, int toIndex)
		{
			return ToEnumerable(fromIndex, toIndex);
		}

        protected override IList NonGenericToList()
		{
			return (IList)ToList();
		}

        protected override IList NonGenericToList(int fromIndex, int toIndex)
		{
			return (IList)ToList(fromIndex, toIndex);
		}

        public virtual Task<IEnumerable<TEntity>> ToEnumerableAsync()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetSelectCommand,
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ToEnumerableAsync<TEntity>();
        }

        public virtual Task<IEnumerable<TEntity>> ToEnumerableAsync(int fromIndex, int toIndex)
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = () => GetSelectCommand(fromIndex, toIndex),
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ToEnumerableAsync<TEntity>();
        }

        public async Task<TEntity> FirstOrDefaultAsync()
        {
            return (await this.ToEnumerableAsync().ConfigureAwait(false)).FirstOrDefault();
        }

        protected override async Task<object> NonGenericFirstOrDefaultAsync()
        {
            return await this.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public virtual Task<List<TEntity>> ToListAsync()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetSelectCommand,
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ToListAsync<TEntity>();
        }

        public virtual Task<List<TEntity>> ToListAsync(int fromIndex, int toIndex)
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = () => GetSelectCommand(fromIndex, toIndex),
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ToListAsync<TEntity>();
        }

        protected override async Task<IEnumerable> NonGenericToEnumerableAsync()
        {
            return await this.ToEnumerableAsync().ConfigureAwait(false);
        }

        protected override async Task<IEnumerable> NonGenericToEnumerableAsync(int fromIndex, int toIndex)
        {
            return await ToEnumerableAsync(fromIndex, toIndex).ConfigureAwait(false);
        }

        protected override async Task<IList> NonGenericToListAsync()
        {
            return (IList) await this.ToListAsync().ConfigureAwait(false);
        }

        protected override async Task<IList> NonGenericToListAsync(int fromIndex, int toIndex)
        {
            return (IList) await ToListAsync(fromIndex, toIndex).ConfigureAwait(false);
        }
    }

}
