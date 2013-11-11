using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Reflection;
using System.Data.Common;
using NLog;
using System.Diagnostics;
using inercya.EntityLite.Extensions;
using inercya.EntityLite.Builders;
using System.Linq;
using inercya.EntityLite.Collections;

namespace inercya.EntityLite
{
    public enum Provider
    {
        SqlClient,
        OracleClient,
        SQLite,
        MySql
    }

    public class ConnectionOpennedEventArgs : EventArgs
    {
        public DbConnection Connection { get; private set; }
        public ConnectionOpennedEventArgs(DbConnection connection)
        {
            this.Connection = connection;
        }

    }


    public class ErrorOcurredEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }
        public bool WillRetry { get; private set; }

        public ErrorOcurredEventArgs(Exception exception, bool willRetry)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            this.Exception = exception;
        }
    }

    

    public partial class DataService : IDisposable
    {

        public object CurrentUserId { get; set; }

		public int MaxRetries { get; protected set; }
		public int InitialMillisecondsRetryDelay { get; protected set; }
		public SpecialFieldNames SpecialFieldNames { get; set; }

        private string _defaultSchemaName;
        public string DefaultSchemaName
        {
            get
            {
                if (_defaultSchemaName == null)
                {
                    if (this.Provider == EntityLite.Provider.SqlClient)
                    {
                        _defaultSchemaName = "dbo";
                    }
                }
                return _defaultSchemaName;
            }
            protected set
            {
                _defaultSchemaName = value;
            }
        }

        private void SetDefaultValues()
        {

			this.MaxRetries = 7;
			this.InitialMillisecondsRetryDelay = 20;
        }

		private readonly CommandBuilder commandBuilder;

		private Logger Log = NLog.LogManager.GetLogger(typeof(DataService).FullName);

        private string _connectionStringName;
        public string ConnectionStringName
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionStringName))
                {
                    return "ConnectionString";
                }
                else
                {
                    return _connectionStringName;
                }
            }
            set
            {
                if (value != this.ConnectionStringName)
                {
                    _connectionStringName = value;
                    _connectionString = null;
                    _providerName = null;
                    _provider = null;
                    _providerFactory = null;
                    _parameterPrefix = null;
                    _connection = null;
                    _transaction = null;
                }
            }
        }

		

        internal void NotifyErrorOcurred(Exception ex, bool willRetry)
        {
            if (ErrorOcurred != null)
            {
                ErrorOcurred(this, new ErrorOcurredEventArgs(ex, willRetry));
            }
        }

        public event EventHandler<ErrorOcurredEventArgs> ErrorOcurred;

        private string _connectionString;
        public  string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    var connectionStringSetting = ConfigurationManager.ConnectionStrings[ConnectionStringName];
                    if (connectionStringSetting != null)
                    {
                        _connectionString = connectionStringSetting.ConnectionString;
                    }
                }
                return _connectionString;
            }
            protected set
            {
                this._connectionString = value;
            }
        }

        private string _providerName;
        public string ProviderName
        {
            get
            {
                if (string.IsNullOrEmpty(_providerName))
                {
					var connectionStringSetting = ConfigurationManager.ConnectionStrings[ConnectionStringName];
					if (connectionStringSetting != null)
					{
						_providerName = connectionStringSetting.ProviderName;
					}
                }
                return _providerName;
            }
            protected set
            {
                this._providerName = value;
            }
        }

        private Provider? _provider;
        public Provider Provider
        {
           get 
           {
               if (_provider == null)
               {
                   switch (ProviderName)
                   {
                       case "System.Data.SqlClient":
                           _provider = Provider.SqlClient;
                           break;
                       case "System.Data.SQLite":
                           _provider = Provider.SQLite;
                           break;
                       case "System.Data.OracleClient":
                           _provider = Provider.OracleClient;
                           break;
                       case "MySql.Data.MySqlClient":
                           _provider = Provider.MySql;
                           break;
                       default:
                           throw new NotSupportedException("The provider " + ProviderName + " is not supported");
                   }
               }
               return _provider.Value;
           }
        }

        private string _parameterPrefix;

        public string ParameterPrefix
        {
            get
            {
                if (string.IsNullOrEmpty(_parameterPrefix))
                {
                    switch (Provider)
                    {
                        case Provider.SqlClient:
                        case Provider.SQLite:
                        case Provider.MySql:
                            _parameterPrefix = "@";
                            break;
                        case Provider.OracleClient:
                            _parameterPrefix = ":";
                            break;
                        default:
                            throw new NotSupportedException("The provider " + ProviderName + " is not supported");
                    }
                }
                return _parameterPrefix;
            }
        }

        private string _startQuote;
        public string StartQuote
        {
            get
            {
                if (_startQuote == null)
                {
                    switch (Provider)
                    {
                        case Provider.SqlClient:
                        case Provider.SQLite:
                            _startQuote = "[";
                            break;
                        case Provider.MySql:
                            _startQuote = "`";
                            break;
                        case Provider.OracleClient:
                            _startQuote = "\"";
                            break;
                        default:
                            throw new NotSupportedException("The provider " + ProviderName + " is not supported");
                    }
                }
                return _startQuote;
            }
        }

        private string _endQuote;
        public string EndQuote
        {
            get
            {
                if (_endQuote == null)
                {
                    switch (Provider)
                    {
                        case Provider.SqlClient:
                        case Provider.SQLite:
                            _endQuote = "]";
                            break;
                        case Provider.MySql:
                            _endQuote = "`";
                            break;
                        case Provider.OracleClient:
                            _endQuote = "\"";
                            break;
                        default:
                            throw new NotSupportedException("The provider " + ProviderName + " is not supported");
                    }
                }
                return _endQuote;
            }
        }

        private static SynchronizedDictionary<string, DbProviderFactory> providerFactories;

        private static DbProviderFactory GetProviderFactory(string provider)
        {
            DbProviderFactory factory = null;
            if (!providerFactories.TryGetValue(provider, out factory))
            {                 
                factory = DbProviderFactories.GetFactory(provider);
                providerFactories[provider] = factory;
            }
            return factory;
        }

        private IIdentityMap _identityMap;

        public IIdentityMap IdentityMap
        {
            get
            {
                if (_identityMap == null)
                {
                    _identityMap = new IdentityMapLite();
                }
                return _identityMap;
            }
            protected set
            {
                _identityMap = value;
            }
        }

        static DataService()
        {
            providerFactories = new SynchronizedDictionary<string, DbProviderFactory>();
        }

        private DbProviderFactory _providerFactory;
        public DbProviderFactory ProviderFactory
        {
            get
            {
                if (_providerFactory == null)
                {
                    _providerFactory = GetProviderFactory(this.ProviderName);
                }
                return _providerFactory;
            }
        }

        private DbConnection _connection;
        public DbConnection Connection 
        {
            get
            {
                if (_connection == null)
                {
                    _connection = ProviderFactory.CreateConnection();
                    _connection.StateChange += new StateChangeEventHandler(_connection_StateChange);
                    if (!string.IsNullOrEmpty(this.ConnectionString))
                    {
                        _connection.ConnectionString = this.ConnectionString;
                    }
                }
                return _connection;
            }
        }

        public event EventHandler<ConnectionOpennedEventArgs> ConnectionOpened;
        

        void _connection_StateChange(object sender, StateChangeEventArgs e)
        {
            if (e.CurrentState == ConnectionState.Open)
            {
                if (ConnectionOpened != null)
                {
                    ConnectionOpened(this, new ConnectionOpennedEventArgs((DbConnection)sender));
                }
            }
        }
       

        private DbTransaction _transaction;
        public DbTransaction Transaction
        {
            get 
            {
                return _transaction;
            }
        }

        public virtual void OpenConnection()
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Open();
            }
        }

        public int TransactionCount { get; private set; }

        public void BeginTransaction()
        {
            OpenConnection();
            if (!IsActiveTransaction)
            {
                _transaction = this.Connection.BeginTransaction();
            }
            TransactionCount++;

        }

        public bool IsActiveTransaction
        {
            get { return this.Transaction != null && this.Transaction.Connection != null; }
        }

        public void Commit()
        {
			try
			{
				if (IsActiveTransaction)
				{
					if (TransactionCount == 1)
					{
						this.Transaction.Commit();
						this.Transaction.Dispose();
						_transaction = null;
					}
					TransactionCount--;
				}
				else
				{
					throw new InvalidOperationException("Cannot commit because there is no active transaction");
				}
			}
			catch (Exception ex)
			{
				Log.ErrorException("Error on commit", ex);
				throw;
			}
        }

        public void Rollback()
        {
			try
			{
				if (IsActiveTransaction)
				{
					this.Transaction.Rollback();
					this.Transaction.Dispose();
					_transaction = null;
					TransactionCount = 0;
				}
				else
				{
					throw new InvalidOperationException("Cannot rollback because there is no active transaction");
				}
			}
			catch (Exception ex)
			{
				Log.ErrorException("Error on Rollback", ex);
				throw;
			}
        }

        public DataService()
        {
			this.commandBuilder = new CommandBuilder(this);
			this.SpecialFieldNames = new SpecialFieldNames();
			this.SetDefaultValues();
        }

        public DataService(string connectionStringName) : this()
        {
            this.ConnectionStringName = connectionStringName;
        }

        public DataService(string connectionString, string providerName) :this()
        {
            this._connectionString = connectionString;
            this._providerName = providerName;
        }


        #region IDisposable Members

        private bool _isDisposed;

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {


                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

		#region Modification methods
		

		public virtual void Delete(object entity)
		{
			try
			{
				if (entity == null) throw new ArgumentNullException("entity");
				DbCommand cmd = this.commandBuilder.GetDeleteCommand(entity);
				int maxRetries = this.IsActiveTransaction ? 0 : MaxRetries;
				FunctionExtensions.ExecuteWithRetries(delegate
				{
					cmd.Connection = this.Connection;
					if (this.IsActiveTransaction)
					{
						cmd.Transaction = this.Transaction;
					}
					this.OpenConnection();
					var watch = Stopwatch.StartNew();
					var result = cmd.ExecuteNonQuery();
					watch.Stop();
					CommandExecutionLogger.LogCommandExecution(cmd, this, (long)(1e6 * watch.Elapsed.Ticks / Stopwatch.Frequency));
					return result;
				}, maxRetries, this.InitialMillisecondsRetryDelay, (ex, willRetry) => this.NotifyErrorOcurred(ex, willRetry));

                    var identity = entity.TryGetId();
                    if (identity != null) IdentityMap.Remove(entity.GetType(), identity);
			}
			catch (Exception ex)
			{
				if (entity == null)
				{
					Log.ErrorException("Error deleting entity", ex);
				}
				else
				{
					Log.ErrorException(string.Format("Error deleting entity of type {0} with id {1}", entity.GetType().Name, entity.GetPrimaryKey().ToListString()), ex);
				}
				throw;
			}
		}

		public void Delete<T>(IEnumerable<T> entities)
		{
			if (entities == null) throw new ArgumentNullException("entitiesCollection");
			//Type entityType = entitiesCollection.GetType();
			foreach (object entity in entities)
			{
				this.Delete(entity);
			}
		}

        private void SetAuditDate(string fieldName, object entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            var entityType = entity.GetType();
            var metadata = EntityMetadata.GetEntityMetadata(entityType);
            var setters = PropertyHelper.GetPropertySetters(entityType);
            PropertySetter setter = null;
            if (!string.IsNullOrEmpty(fieldName) && setters.TryGetValue(fieldName, out setter))
            {
                Type type = metadata.Properties[fieldName].PropertyInfo.PropertyType;
                if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    setter(entity, DateTime.Now);
                }
                else if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
                {
                    setter(entity, DateTimeOffset.Now);
                }
                else
                {
                    throw new NotSupportedException("The field \"" + fieldName + "\" is of an unsuported type " + type.Name);
                }
            }
        }

        private void SetAuditUser(string fieldName, object entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (this.CurrentUserId == null) return;
            var entityType = entity.GetType();
            var setters = PropertyHelper.GetPropertySetters(entityType);
            PropertySetter setter = null;
            if (!string.IsNullOrEmpty(fieldName) && setters.TryGetValue(fieldName, out setter))
            {
                setter(entity, this.CurrentUserId);
            }
        }

		public virtual void Insert(object entity)
		{
			try
			{
				if (entity == null) throw new ArgumentNullException("entity");

				Type entityType = entity.GetType();
                var setters = entityType.GetPropertySetters();

                SetAuditDate(this.SpecialFieldNames.CreatedDateFieldName, entity);
                SetAuditDate(this.SpecialFieldNames.ModifiedDateFieldName, entity);
                SetAuditUser(this.SpecialFieldNames.CreatedByFieldName, entity);
                SetAuditUser(this.SpecialFieldNames.ModifiedByFieldName, entity);

				DbCommand cmd = this.commandBuilder.GetInsertCommand(entity);
				int maxRetries = this.IsActiveTransaction ? 0 : MaxRetries;

				FunctionExtensions.ExecuteWithRetries(delegate
				{
					cmd.Connection = this.Connection;
					if (this.IsActiveTransaction)
					{
						cmd.Transaction = this.Transaction;
					}
					this.OpenConnection();
					var watch = Stopwatch.StartNew();
					using (IDataReader reader = cmd.ExecuteReader())
					{
						EntityMetadata entityMetadata = entity.GetType().GetEntityMetadata();
						string autoIncrementFieldName = entityMetadata.AutoIncrementFieldName;

						if (!string.IsNullOrEmpty(autoIncrementFieldName) && reader.Read())
						{
							int fieldOrdinal = 0;
							Type fieldType = reader.GetFieldType(fieldOrdinal);
							Type propertyType = entityMetadata.Properties[autoIncrementFieldName].PropertyInfo.PropertyType;
							if (fieldType == propertyType)
							{
								setters[autoIncrementFieldName](entity, reader.GetValue(fieldOrdinal));
							}
							else
							{
								setters[autoIncrementFieldName](entity, Convert.ChangeType(reader.GetValue(fieldOrdinal), propertyType));
							}
						}
						while (reader.NextResult()) ;

					}
					watch.Stop();
					CommandExecutionLogger.LogCommandExecution(cmd, this, (long)(1e6 * watch.Elapsed.Ticks / Stopwatch.Frequency));
					return 0;
				}, maxRetries, this.InitialMillisecondsRetryDelay, (ex, willRetry) => this.NotifyErrorOcurred(ex, willRetry));
			}
			catch (Exception ex)
			{
				if (entity == null)
				{
					Log.ErrorException("Error inserting entity", ex);
				}
				else
				{
					Log.ErrorException(string.Format("Error inserting entity of type {0} with primary key {1} ", entity.GetType().Name, entity.GetPrimaryKey().ToListString()), ex);
				}
				throw;
			}
		}

        public virtual int Update(object entity)
        {
            return Update(entity, GetValidatedForUpdateSortedFields(entity));
        }

        public int Update(object entity, params string[] fieldsToUpdate)
        {
            return Update(entity, GetValidatedForUpdateSortedFields(entity, fieldsToUpdate));
        }



        protected virtual int Update(object entity, List<string> sortedFields)
        {
            try
            {
                if (entity == null) throw new ArgumentNullException("entity");
                Type entityType = entity.GetType();
                var metadata = entityType.GetEntityMetadata();
               
                SetAuditUser(this.SpecialFieldNames.ModifiedByFieldName, entity);
                SetAuditDate(this.SpecialFieldNames.ModifiedDateFieldName, entity);

                DbCommand cmd = this.commandBuilder.GetUpdateCommand(entity, sortedFields);

                int maxRetries = this.IsActiveTransaction ? 0 : MaxRetries;
                var affectedRecords = FunctionExtensions.ExecuteWithRetries(delegate
                {
                    cmd.Connection = this.Connection;
                    if (this.IsActiveTransaction)
                    {
                        cmd.Transaction = this.Transaction;
                    }
                    this.OpenConnection();
                    var watch = Stopwatch.StartNew();
                    int result = cmd.ExecuteNonQuery();
                    watch.Stop();
                    CommandExecutionLogger.LogCommandExecution(cmd, this, (long)(1e6 * watch.Elapsed.Ticks / Stopwatch.Frequency));
                    return result;

                }, maxRetries, this.InitialMillisecondsRetryDelay, (ex, willRetry) => this.NotifyErrorOcurred(ex, willRetry));
                var identity = entity.TryGetId();
                if (identity != null) IdentityMap.Remove(entityType, identity);
                return affectedRecords;
            }
            catch (Exception ex)
            {
                if (entity == null)
                {
                    Log.ErrorException("Error updating entity", ex);
                }
                else
                {
                    Log.ErrorException(string.Format("Error updating entity of type {0} with primary key: {1}", entity.GetType().Name, entity.GetPrimaryKey().ToListString()), ex);
                }
                throw;
            }
        }

        protected List<string> GetValidatedForUpdateSortedFields(object entity, string[] fieldsToUpdate = null)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();
            var metadata = entityType.GetEntityMetadata();

            if (string.IsNullOrEmpty(metadata.BaseTableName))
            {
                throw new InvalidOperationException(string.Format("Entity \"{0}\" is not updatable because it has no base table", entityType.Name));
            }
            List<string> sortedFields = null;

            if (fieldsToUpdate == null)
            {
                return metadata.UpdatableProperties
                    .Select(kv => kv.Key)
                    .Where(x => !metadata.PrimaryKeyPropertyNames.Contains(x, StringComparer.InvariantCultureIgnoreCase))
                    .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase).ToList();
            }

            sortedFields = new List<string>(fieldsToUpdate);
            sortedFields.Sort(StringComparer.InvariantCultureIgnoreCase);
            sortedFields.EnsureField(metadata, SpecialFieldNames.ModifiedByFieldName);
            sortedFields.EnsureField(metadata, SpecialFieldNames.ModifiedDateFieldName);
            sortedFields.EnsureField(metadata, SpecialFieldNames.EntityRowVersionFieldName);

            foreach (var field in sortedFields)
            {
                if (metadata.PrimaryKeyPropertyNames.Contains(field, StringComparer.InvariantCultureIgnoreCase))
                {
                    throw new NotSupportedException(string.Format("Updating primary key fields is an unsupported operation. And you are trying to update the primary key field \"{0}\"", field));
                }
                if (!metadata.UpdatableProperties.ContainsKey(field))
                {
                    throw new InvalidOperationException(string.Format("\"{0}\" is not an updatable field of table \"{0}\"", field, metadata.BaseTableName));
                }
            }
            return sortedFields;
        }

		#endregion
	}
}
