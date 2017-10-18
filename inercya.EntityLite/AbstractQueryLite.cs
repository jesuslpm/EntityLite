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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace inercya.EntityLite
{
    [Serializable]
    public abstract class AbstractQueryLite : IQueryLite
    {

        private static ILogger logger;

        private static ILogger Log
        {
            get
            {
                if (logger == null)
                {
                    logger = ConfigurationLite.LoggerFactory.CreateLogger<AbstractQueryLite>();
                }
                return logger;
            }
        }
        public IList<string> FieldList { get; set; }
        public ICollection<ConditionLite> Filter { get; set; }
        public ICollection<SortDescriptor> Sort { get; set; }
		public ICollection<string> Options { get; set; }

        [NonSerialized]
        private DataService _dataService;
        public DataService DataService { get { return _dataService; } set { _dataService = value; } }

        [NonSerialized]
        private IQueryBuilder _queryBuilder;
        public IQueryBuilder QueryBuilder { get { return _queryBuilder; } set { _queryBuilder = value; } }

        public int CommandTimeout { get; set; }


		protected abstract IEnumerable NonGenericToEnumerable();
        protected abstract Task<IEnumerable> NonGenericToEnumerableAsync();

        IEnumerable IQueryLite.ToEnumerable()
		{
			return NonGenericToEnumerable();
		}

        Task<IEnumerable> IQueryLite.ToEnumerableAsync()
        {
            return NonGenericToEnumerableAsync();
        }

		protected abstract IEnumerable NonGenericToEnumerable(int fromIndex, int toIndex);

        protected abstract Task<IEnumerable> NonGenericToEnumerableAsync(int fromIndex, int toIndex);

        IEnumerable IQueryLite.ToEnumerable(int fromIndex, int toIndex)
		{
			return NonGenericToEnumerable(fromIndex, toIndex);
		}

        Task<IEnumerable> IQueryLite.ToEnumerableAsync(int fromIndex, int toIndex)
        {
            return NonGenericToEnumerableAsync(fromIndex, toIndex);
        }

        IList IQueryLite.ToList()
        {
            return NonGenericToList();
        }

        Task<IList> IQueryLite.ToListAsync()
        {
            return NonGenericToListAsync();
        }

        protected abstract IList NonGenericToList();
        protected abstract Task<IList> NonGenericToListAsync();

        IList IQueryLite.ToList(int fromIndex, int toIndex)
        {
            return NonGenericToList(fromIndex, toIndex);
        }

        Task<IList> IQueryLite.ToListAsync(int fromIndex, int toIndex)
        {
            return NonGenericToListAsync(fromIndex, toIndex);
        }

        protected abstract IList NonGenericToList(int fromIndex, int toIndex);

        protected abstract Task<IList> NonGenericToListAsync(int fromIndex, int toIndex);

        protected abstract object NonGenericFirstOrDefault();

        protected abstract Task<object> NonGenericFirstOrDefaultAsync();

        object IQueryLite.FirstOrDefault()
		{
			return NonGenericFirstOrDefault();
		}

        Task<object> IQueryLite.FirstOrDefaultAsync()
        {
            return NonGenericFirstOrDefaultAsync();
        }

        [NonSerialized]
        private Type _entityType;
        public virtual Type EntityType { get { return _entityType; } set { _entityType = value; } }


        protected DbCommand GetSelectCommand()
        {
            DbCommand selectCommand = this.DataService.EntityLiteProvider.CreateCommand();
            int paramIndex = 0;
            selectCommand.CommandText = QueryBuilder.GetSelectQuery(selectCommand, ref paramIndex);
            return selectCommand;
        }

        protected DbCommand GetSelectCommand(int fromIndex, int toIndex)
        {
            DbCommand selectCommand = this.DataService.EntityLiteProvider.CreateCommand();
            int paramIndex = 0;
            selectCommand.CommandText = QueryBuilder.GetSelectQuery(selectCommand, ref paramIndex, fromIndex, toIndex);
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
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public async Task<int> GetCountAsync()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetCountCommand,
                CommandTimeout = this.CommandTimeout
            };
            object result = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
            return Convert.ToInt32(result);
        }

        public bool Any()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetAnyCommand,
                CommandTimeout = this.CommandTimeout
            };

            return Convert.ToInt32(cmd.ExecuteScalar()) == 1; 
        }

        public async Task<bool> AnyAsync()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetAnyCommand,
                CommandTimeout = this.CommandTimeout
            };
            var result = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
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
    }

    public class AbstractQueryLite<TEntity> : AbstractQueryLite, IQueryLite<TEntity> where TEntity : class, new()
    {

        private static ILogger logger;

        private static ILogger Log
        {
            get
            {
                if (logger == null)
                {
                    logger = ConfigurationLite.LoggerFactory.CreateLogger("inercya.EntityLite.QueryLite<" + typeof(TEntity).Name + ">");
                }
                return logger;
            }
        }

        protected AbstractQueryLite() : base()
        {
            this.EntityType = typeof(TEntity);
        }

        #region IQueryLite<TEntity> Members


		public virtual IEnumerable<TEntity> ToEnumerable()
		{
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetSelectCommand,
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ToEnumerable<TEntity>();
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

        public virtual IEnumerable<TEntity> ToEnumerable(int fromIndex, int toIndex)
		{
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = () => GetSelectCommand(fromIndex, toIndex),
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ToEnumerable<TEntity>();
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

        public TEntity FirstOrDefault()
		{
			return this.ToEnumerable().FirstOrDefault();
		}

        public async Task<TEntity> FirstOrDefaultAsync()
        {
            return (await this.ToEnumerableAsync().ConfigureAwait(false)).FirstOrDefault();
        }

        protected override object NonGenericFirstOrDefault()
		{
			return this.FirstOrDefault();
		}

        protected override async Task<object> NonGenericFirstOrDefaultAsync()
        {
            return await this.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public virtual IList<TEntity> ToList()
		{
			return ToEnumerable().ToList();
		}

        public virtual Task<IList<TEntity>> ToListAsync()
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = GetSelectCommand,
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ToListAsync<TEntity>();
        }

        public virtual IList<TEntity> ToList(int fromIndex, int toIndex)
        {
			return ToEnumerable(fromIndex, toIndex).ToList();
        }

        public virtual Task<IList<TEntity>> ToListAsync(int fromIndex, int toIndex)
        {
            var cmd = new CommandExecutor(this.DataService, true)
            {
                GetCommandFunc = () => GetSelectCommand(fromIndex, toIndex),
                CommandTimeout = this.CommandTimeout
            };
            return cmd.ToListAsync<TEntity>();
        }


        protected override IEnumerable NonGenericToEnumerable()
		{
			return ToEnumerable();
		}

        protected override async Task<IEnumerable> NonGenericToEnumerableAsync()
        {
            return await this.ToEnumerableAsync().ConfigureAwait(false);
        }


        protected override IEnumerable NonGenericToEnumerable(int fromIndex, int toIndex)
		{
			return ToEnumerable(fromIndex, toIndex);
		}

        protected override async Task<IEnumerable> NonGenericToEnumerableAsync(int fromIndex, int toIndex)
        {
            return await ToEnumerableAsync(fromIndex, toIndex).ConfigureAwait(false);
        }

        protected override IList NonGenericToList()
		{
			return (IList)ToList();
		}

        protected override async Task<IList> NonGenericToListAsync()
        {
            return (IList) await this.ToListAsync().ConfigureAwait(false);
        }


        protected override IList NonGenericToList(int fromIndex, int toIndex)
		{
			return (IList)ToList(fromIndex, toIndex);
		}

        protected override async Task<IList> NonGenericToListAsync(int fromIndex, int toIndex)
        {
            return (IList) await ToListAsync(fromIndex, toIndex).ConfigureAwait(false);
        }
        #endregion
    }

}
