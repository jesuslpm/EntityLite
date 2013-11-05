using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.Common;
using inercya.EntityLite.Extensions;
using NLog;
using System.Diagnostics;
using inercya.EntityLite.Builders;

namespace inercya.EntityLite
{
    [Serializable]
    public abstract class AbstractQueryLite : IQueryLite
    {

        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        public string FieldList { get; set; }
        public ICollection<SimpleConditionLite> Filter { get; set; }
        public ICollection<SortDescriptor> Sort { get; set; }
		public ICollection<string> Options { get; set; }

        [NonSerialized]
        private DataService _dataService;
        public DataService DataService { get { return _dataService; } set { _dataService = value; } }

        [NonSerialized]
        Func<IQueryBuilder> _createQueryBuilder;

        public Func<IQueryBuilder> CreateQueryBuilder { get { return _createQueryBuilder; } set { _createQueryBuilder = value; } }


        private IQueryBuilder GetQueryBuilder()
        {
            if (CreateQueryBuilder == null)
            {
                throw new InvalidOperationException("CreateQueryBuilder function must be set");
            }
            return CreateQueryBuilder();
        }

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
            return NonGenericToList();
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


        protected DbCommand GetSelectCommand()
        {
            DbCommand selectCommand = this.DataService.Connection.CreateCommand();
            int paramIndex = 0;
            selectCommand.CommandText = GetQueryBuilder().GetSelectQuery(selectCommand, ref paramIndex);
            return selectCommand;
        }

        protected DbCommand GetSelectCommand(int fromIndex, int toIndex)
        {
            DbCommand selectCommand = this.DataService.Connection.CreateCommand();
            int paramIndex = 0;
            selectCommand.CommandText = GetQueryBuilder().GetSelectQuery(selectCommand, ref paramIndex, fromIndex, toIndex);
            return selectCommand;
        }

        protected DbCommand GetCountCommand()
        {
            DbCommand selectCommand = this.DataService.Connection.CreateCommand();
            int paramIndex = 0;
            selectCommand.CommandText =GetQueryBuilder().GetCountQuery(selectCommand, ref paramIndex);
            return selectCommand;
        }

		protected T ExecuteCommand<T>(Func<DbCommand> createCommand, Func<Func<DbCommand>, T> executeCommandFunc)
		{
			try
			{
				int maxRetries = this.DataService.IsActiveTransaction ? 0 : DataService.MaxRetries;
				var watch = new Stopwatch();
				DbCommand command =  null;
				Func<T> func = () => 
				{

					this.DataService.OpenConnection();
					return executeCommandFunc(() => {
						command = createCommand();
						command.Connection = this.DataService.Connection;
						if (this.DataService.IsActiveTransaction)
						{
							command.Transaction = this.DataService.Transaction;
						}
						return command;
					});
				};
				var result = func.ExecuteWithRetries(
						maxRetries, DataService.InitialMillisecondsRetryDelay,
						(ex, willRetry) => this.DataService.NotifyErrorOcurred(ex, willRetry));
				CommandExecutionLogger.LogCommandExecution(command, this.DataService, (long)(1e6 * watch.Elapsed.Ticks / Stopwatch.Frequency));
				return result;
			}
			catch (Exception ex)
			{
				Log.ErrorException("Couldn't execute command", ex);
				throw;
			}
		}

		protected IEnumerable<T> ToEnumerable<T>(Func<DbCommand> createCommand)
		{
			return ExecuteCommand(createCommand, ToEnumerableImplementation<T>);
		}

		private static IEnumerable<T> ToEnumerableImplementation<T>(Func<DbCommand> createCommand)
		{
			using (var cmd = createCommand())
			using (var reader = cmd.ExecuteReader())
			{
				var factory = reader.GetFactory(typeof(T));
				while (reader.Read())
				{
					yield return (T)factory(reader);
				}
			}
		}


        public int GetCount()
        {
			return ExecuteCommand(GetCountCommand, (createCmd) => 
			{
				using (var cmd = createCmd())
				{
					return Convert.ToInt32(cmd.ExecuteScalar());
				}
			});
        }

        protected AbstractQueryLite()
        {
            this.Filter = new List<SimpleConditionLite>();
            this.Sort = new List<SortDescriptor>();
			this.Options = new List<string>();
            this.FieldList = "*";
        }
    }

    public class AbstractQueryLite<TEntity> : AbstractQueryLite, IQueryLite<TEntity> where TEntity : class, new()
    {
        private static readonly Logger Log = LogManager.GetLogger("inercya.EntityLite.QueryLite<" + typeof(TEntity).Name + ">");

        protected AbstractQueryLite() : base()
        {
            this.EntityType = typeof(TEntity);
        }

        #region IQueryLite<TEntity> Members


		public virtual IEnumerable<TEntity> ToEnumerable()
		{
			return base.ToEnumerable<TEntity>(GetSelectCommand);
		}

		public virtual IEnumerable<TEntity> ToEnumerable(int fromIndex, int toIndex)
		{
			return base.ToEnumerable<TEntity>(() => GetSelectCommand(fromIndex, toIndex));
		}

		public TEntity FirstOrDefault()
		{
			return this.ToEnumerable().FirstOrDefault();
		}

		protected override object NonGenericFirstOrDefault()
		{
			return this.FirstOrDefault();
		}

		public virtual IList<TEntity> ToList()
		{
			return ToEnumerable().ToList();
		}

        public virtual IList<TEntity> ToList(int fromIndex, int toIndex)
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
        #endregion

    }

}
