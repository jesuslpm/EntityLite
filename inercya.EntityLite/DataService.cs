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
using System.Text;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Reflection;
using System.Data.Common;
using System.Diagnostics;
using inercya.EntityLite.Extensions;
using inercya.EntityLite.Builders;
using System.Linq;
using inercya.EntityLite.Collections;
using inercya.EntityLite.Providers;
#if (NET452 || NETSTANDARD2_0)
using System.Threading.Tasks;
#endif
using Microsoft.Extensions.Logging;

namespace inercya.EntityLite
{

    public partial class DataService : IDisposable
    {

        public static readonly IDictionary<string, Func<DataService, IEntityLiteProvider>> EntityLiteProviderFactories = new Dictionary<string, Func<DataService, IEntityLiteProvider>>();

        private static readonly CacheLite<string, ConnectionStringSettings> connectionStrings = new CacheLite<string, ConnectionStringSettings>((name) => ConfigurationManager.ConnectionStrings[name]);

        private object _currentUserId;


        public Func<object> CurrentUserIdGetter { get; set; }

#if (NET452 || NETSTANDARD2_0)
        public Func<Task<object>> CurrentUserIdAsyncGetter { get; set; }
#endif

        public object CurrentUserId 
        { 
            get 
            {
                if (CurrentUserIdGetter != null) return CurrentUserIdGetter();
#if (NET452 || NETSTANDARD2_0)
                if (CurrentUserIdAsyncGetter != null) return CurrentUserIdAsyncGetter().Result;
#endif
                return _currentUserId;
            }
            set
            {
                _currentUserId = value;
                CurrentUserIdGetter = null;
#if (NET452 || NETSTANDARD2_0)
                CurrentUserIdAsyncGetter = null;
#endif
            }
        }

#if (NET452 || NETSTANDARD2_0)
        public Task<object> GetCurrentUserIdAsync()
        {
            if (CurrentUserIdAsyncGetter != null) return CurrentUserIdAsyncGetter();
            return Task.FromResult(CurrentUserId);
        }
#endif

		public int MaxRetries { get; protected set; }
		public int InitialMillisecondsRetryDelay { get; protected set; }
		public SpecialFieldNames SpecialFieldNames { get; set; }

        private IEntityLiteProvider _entityLiteProvider;

        public int CommandTimeout { get; set; }

        public DateTimeKind AuditDateTimeKind { get; set; }

        public IEntityLiteProvider EntityLiteProvider
        {
            get
            {
                if (_entityLiteProvider == null)
                {
                    if (string.IsNullOrEmpty(this.ProviderName))
                    {
                        throw new InvalidOperationException("The ProviderName property of the data service has not been set. Please check that the connection string \"" + this.ConnectionStringName + "\"  exists in the configuration file and it includes the attribute ProviderName");
                    }
                    Func<DataService, IEntityLiteProvider> factory = null;
                    if (!EntityLiteProviderFactories.TryGetValue(this.ProviderName, out factory))
                    {
                        throw new InvalidOperationException("There is no registered EntityLite provider for: " + this.ProviderName);
                    }
                    _entityLiteProvider = factory(this);
                }
                return _entityLiteProvider;
            }
        }

        public TextTransform EntityNameToEntityViewTransform { get; set; }

		private CommandBuilder commandBuilder;

        private static ILogger logger;

        private static ILogger Log
        {
            get
            {
                if (logger == null)
                {
                    logger = ConfigurationLite.LoggerFactory.CreateLogger<DataService>();
                }
                return logger;
            }
        }

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
                    if (!string.IsNullOrEmpty(value))
                    {
                        ConnectionStringSettings settings = connectionStrings.GetItem(value);
                        if (settings == null)
                        {
                            throw new ArgumentException("The connection string \"" + value + "\" doesn't exist in the configuration file");
                        }
                        if (string.IsNullOrEmpty(settings.ProviderName))
                        {
                            throw new ArgumentException("Invalid connection string \"" + value + "\" in configuration file. The providerName attribute is missing or empty");
                        }
                        if (string.IsNullOrEmpty(settings.ConnectionString))
                        {
                            throw new ArgumentException("Invalid connection string \"" + value + "\" in configuration file. The connectionString attribute is missing or empty");
                        }
                        _connectionStringName = value;
                        _connectionString = settings.ConnectionString;
                        _providerName = settings.ProviderName;
                    }
                    else
                    {
                        _connectionStringName = value;
                        _connectionString = null;
                        _providerName = null;
                    }
                    _dbProviderFactory = null;
                    _entityLiteProvider = null;
                    _connection = null;
                    _transaction = null;
                    this.TransactionCount = 0;
                }
            }
        }

        public bool IsAutomaticAuditDateFieldsEnabled { get; set; }
        public bool IsAutomaticAuditUserFieldsEnabled { get; set; }

        internal void NotifyErrorOcurred(Exception ex, bool willRetry)
        {
            if (ErrorOcurred != null)
            {
                ErrorOcurred(this, new ErrorOcurredEventArgs(ex, willRetry));
            }
        }

        public event EventHandler<ErrorOcurredEventArgs> ErrorOcurred;
        public event EventHandler BeforeCommit;
        public event EventHandler AfterCommit;
        public event EventHandler BeforeRollback;
        public event EventHandler AfterRollback;

        private string _connectionString;
        public  string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    var connectionStringSetting = connectionStrings.GetItem(ConnectionStringName);
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
            EntityLiteProviderFactories.Add(SqlServerEntityLiteProvider.ProviderName, (ds) => new SqlServerEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add(SqliteEntityLiteProvider.ProviderName, (ds) => new SqliteEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add(MySqlEntityLiteProvider.ProviderName, (ds) => new MySqlEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add(OracleEntityLiteProvider.ProviderName, (ds) => new OracleEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add(OracleEntityLiteProvider.ManagedProviderName, (ds) => new OracleEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add(DevArtEntityLiteProvider.ProviderName, (ds) => new DevArtEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add(NpgsqlEntityLiteProvider.ProviderName, (ds) => new NpgsqlEntityLiteProvider(ds));
        }

        private DbProviderFactory _dbProviderFactory;
        public DbProviderFactory DbProviderFactory
        {
            get
            {
                if (_dbProviderFactory == null)
                {
                    if (string.IsNullOrEmpty(this.ProviderName))
                    {
                        throw new InvalidOperationException("The ProviderName property of the data service has not been set. Please check the connection string \"" + this.ConnectionStringName + "\" in the configuration file exists and includes the attribute ProviderName");
                    }
                    _dbProviderFactory = ConfigurationLite.DbProviderFactories.Get(this.ProviderName);
                }
                return _dbProviderFactory;
            }
        }

        private DbConnection _connection;
        public DbConnection Connection 
        {
            get
            {
                CheckDisposed();
                if (_connection == null)
                {
                    if (string.IsNullOrEmpty(this.ConnectionString))
                    {
                        throw new InvalidOperationException("The ConnectionString property of the data service has not been set. Please check that the connection string \"" + this.ConnectionStringName + "\"  exists in the configuration file");
                    }
                    _connection = DbProviderFactory.CreateConnection();
                    _connection.StateChange += new StateChangeEventHandler(_connection_StateChange);
                    _connection.ConnectionString = this.ConnectionString;
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
                CheckDisposed();
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

#if (NET452 || NETSTANDARD2_0)
        private static Task completedTask = Task.FromResult((object)null);

        public virtual Task OpenConnectionAsync()
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                return this.Connection.OpenAsync();
            }
            return completedTask;
        }
#endif

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

        private void CheckDisposed()
        {
            if (this._isDisposed) throw new ObjectDisposedException("DataService");
        }

        public virtual void Commit()
        {
			try
			{
				if (IsActiveTransaction)
				{
                    if (TransactionCount == 1)
                    {
                        this.BeforeCommit?.Invoke(this, EventArgs.Empty);
                        this.Transaction.Commit();
                        this.Transaction.Dispose();
                        _transaction = null;
                        TransactionCount = 0;
                        this.AfterCommit?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        TransactionCount--;
                    }
				}
				else
				{
					throw new InvalidOperationException("Cannot commit because there is no active transaction");
				}
			}
			catch (Exception ex)
			{
				Log.LogError(ex, "Error on commit");
				throw;
			}
        }

        public virtual void Rollback()
        {
			try
			{
				if (IsActiveTransaction)
				{
                    this.BeforeRollback?.Invoke(this, EventArgs.Empty);
					this.Transaction.Rollback();
					this.Transaction.Dispose();
					_transaction = null;
					TransactionCount = 0;
                    this.AfterRollback?.Invoke(this, EventArgs.Empty);
				}
				else
				{
					throw new InvalidOperationException("Cannot rollback because there is no active transaction");
				}
			}
			catch (Exception ex)
			{
				Log.LogError(ex, "Error on Rollback");
				throw;
			}
        }

        public DataService()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.EntityNameToEntityViewTransform = TextTransform.None;
            this.commandBuilder = new CommandBuilder(this);
            this.SpecialFieldNames = new SpecialFieldNames();
            this.MaxRetries = 2;
            this.InitialMillisecondsRetryDelay = 20;
            this.IsAutomaticAuditDateFieldsEnabled = true;
            this.IsAutomaticAuditUserFieldsEnabled = true;
            this.CommandTimeout = -1;
        }

        public DataService(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
            {
                throw new ArgumentNullException("connectionStringName");
            }
            
            this.ConnectionStringName = connectionStringName;
            Initialize();
        }

        public DataService(string connectionString, string providerName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }
            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentNullException("providerName");
            }
            this._connectionString = connectionString;
            this._providerName = providerName;
            Initialize();
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
		

		protected internal virtual bool Delete(object entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();

            var cmd = new CommandExecutor(this, false)
            {
                GetCommandFunc = () => this.commandBuilder.GetDeleteCommand(entity, EntityMetadata.GetEntityMetadata(entityType))
            };
            var affectedRecords = cmd.ExecuteNonQuery();

			var identity = entity.TryGetId();
            if (identity != null) IdentityMap.Remove(entity.GetType(), identity);
            return affectedRecords > 0;
		}

#if (NET452 || NETSTANDARD2_0)
        protected internal virtual async Task<bool> DeleteAsync(object entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();

            var cmd = new CommandExecutor(this, false)
            {
                GetCommandFunc = () => this.commandBuilder.GetDeleteCommand(entity, EntityMetadata.GetEntityMetadata(entityType))
            };
            var affectedRecords = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

            var identity = entity.TryGetId();
            if (identity != null) IdentityMap.Remove(entity.GetType(), identity);
            return affectedRecords > 0;
        }
#endif


        internal struct SetAuditObjectResult
        {
            public bool IsSet;
            public object PreviousValue;
        }

        private SetAuditObjectResult  TrySetAuditUser(string fieldName, object entity, EntityMetadata entityMetadata)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            object previousValue = null;
            if (entityMetadata.Getters.TryGetValue(fieldName, out PropertyGetter getter))
            {
                previousValue = getter(entity);
            }
            if (string.IsNullOrEmpty(fieldName) || !entityMetadata.Setters.TryGetValue(fieldName, out PropertySetter setter))
            {
                return new SetAuditObjectResult { IsSet = false, PreviousValue = previousValue };
            }
            object currentUserId = this.CurrentUserId;
            if (currentUserId == null) return  new SetAuditObjectResult { IsSet = false, PreviousValue = previousValue };
            setter(entity, currentUserId);
            return new SetAuditObjectResult { IsSet = true, PreviousValue = previousValue }; ;
        }

#if (NET452 || NETSTANDARD2_0)
        private async Task<SetAuditObjectResult> TrySetAuditUserAsync(string fieldName, object entity, EntityMetadata entityMetadata)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            object previousValue = null;
            if (entityMetadata.Getters.TryGetValue(fieldName, out PropertyGetter getter))
            {
                previousValue = getter(entity);
            }
            if (string.IsNullOrEmpty(fieldName) || !entityMetadata.Setters.TryGetValue(fieldName, out PropertySetter setter))
            {
                return new SetAuditObjectResult { IsSet = false, PreviousValue = previousValue };
            }
            object currentUserId = await this.GetCurrentUserIdAsync().ConfigureAwait(false);
            if (currentUserId == null) return new SetAuditObjectResult { IsSet = false, PreviousValue = previousValue }; ;
            setter(entity, currentUserId);
            return new SetAuditObjectResult { IsSet = true, PreviousValue = previousValue };
        }
#endif

        private SetAuditObjectResult TrySetAuditDate(string fieldName, object entity, EntityMetadata metadata)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            object previousValue = null;
            if (metadata.Getters.TryGetValue(fieldName, out PropertyGetter getter))
            {
                previousValue = getter(entity);
            }
            if (!string.IsNullOrEmpty(fieldName) && metadata.Setters.TryGetValue(fieldName, out PropertySetter setter))
            {
                Type type = metadata.Properties[fieldName].PropertyInfo.PropertyType;
                if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    if (this.AuditDateTimeKind == DateTimeKind.Utc)
                    {
                        setter(entity, DateTime.UtcNow);
                    }
                    else
                    {
                        setter(entity, DateTime.Now);
                    }
                }
                else if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
                {
                    if (this.AuditDateTimeKind == DateTimeKind.Utc)
                    {
                        setter(entity, DateTimeOffset.UtcNow);
                    }
                    else
                    {
                        setter(entity, DateTimeOffset.Now);
                    }
                }
                else if (type == typeof(string))
                {
                    setter(entity, DateTimeOffset.Now.ToString("O"));
                }
                else
                {
                    throw new NotSupportedException("The field \"" + fieldName + "\" is of an unsuported type " + type.Name);
                }
                return new SetAuditObjectResult { IsSet = true, PreviousValue = previousValue };
            }
            return new SetAuditObjectResult { IsSet = false, PreviousValue = previousValue };
        }



        protected internal virtual Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        protected internal virtual void Insert(object entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();
            Insert(entity, EntityMetadata.GetEntityMetadata(entityType));
        }

#if (NET452 || NETSTANDARD2_0)
        protected internal virtual Task InsertAsync(object entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();
            return InsertAsync(entity, EntityMetadata.GetEntityMetadata(entityType));
        }
#endif

        protected internal virtual void Insert(object entity, EntityMetadata entityMetadata)
		{
		    if (entity == null) throw new ArgumentNullException("entity");

            if (IsAutomaticAuditDateFieldsEnabled)
            {
                TrySetAuditDate(this.SpecialFieldNames.CreatedDateFieldName, entity, entityMetadata);
                TrySetAuditDate(this.SpecialFieldNames.ModifiedDateFieldName, entity, entityMetadata);
            }
            if (IsAutomaticAuditUserFieldsEnabled)
            {
                TrySetAuditUser(this.SpecialFieldNames.CreatedByFieldName, entity, entityMetadata);
                TrySetAuditUser(this.SpecialFieldNames.ModifiedByFieldName, entity, entityMetadata);
            }

            DbCommand cmd = null;

            var cmdExecutor = new CommandExecutor(this, false)
            {
                GetCommandFunc = () => cmd = this.commandBuilder.GetInsertCommand(entity, entityMetadata)
            };

            object autogeneratedFieldValue = null;
            string autogeneratedFieldName = entityMetadata.SequenceFieldName;
            if (entityMetadata.AutoIncrementFieldName != null) autogeneratedFieldName = entityMetadata.AutoIncrementFieldName;
            if (autogeneratedFieldName == null)
            {
                cmdExecutor.ExecuteNonQuery();
                if (entityMetadata.IsPrimaryKeyGuid)
                {
                    Guid primaryKeyValue = (Guid)entity.GetPropertyValue(entityMetadata.PrimaryKeyPropertyName);
                    if (primaryKeyValue == Guid.Empty)
                    {
                        entity.SetPropertyValue(entityMetadata.PrimaryKeyPropertyName, cmd.Parameters[EntityLiteProvider.ParameterPrefix + entityMetadata.PrimaryKeyPropertyName].Value);
                    }
                }
            }
            else
            {
                if (EntityLiteProvider.AutoGeneratedFieldFetchMode == AutoGeneratedFieldFetchMode.OutputParameter)
                {
                    cmdExecutor.SetOutPutParametersAction = (dbcmd) =>
                    {
                        var autogeneratedFieldParam = dbcmd.Parameters.Cast<IDataParameter>().FirstOrDefault(x => x.Direction == ParameterDirection.Output);
                        if (autogeneratedFieldParam == null)
                        {
                            throw new InvalidOperationException(string.Format("There is no output parameter in insert command for autogenerated field {0}.{1},", entityMetadata.EntityType.Name, autogeneratedFieldName));
                        }
                        autogeneratedFieldValue = autogeneratedFieldParam.Value;
                    };
                    cmdExecutor.ExecuteNonQuery();
                }
                else
                {
                    autogeneratedFieldValue = cmdExecutor.ExecuteScalar();
                }
                Type autogeneratedFieldType = autogeneratedFieldValue.GetType();
                if (autogeneratedFieldType == entityMetadata.PrimaryKeyType)
                {
                    entityMetadata.Setters[autogeneratedFieldName](entity, autogeneratedFieldValue);
                }
                else
                {
                    entityMetadata.Setters[autogeneratedFieldName](entity, Convert.ChangeType(autogeneratedFieldValue, entityMetadata.PrimaryKeyType));
                }
            }
            if (entityMetadata.Properties.ContainsKey(SpecialFieldNames.EntityRowVersionFieldName))
            {
                entityMetadata.Setters[SpecialFieldNames.EntityRowVersionFieldName](entity, Convert.ChangeType(1, entityMetadata.Properties[SpecialFieldNames.EntityRowVersionFieldName].PropertyInfo.PropertyType));
            }
		}

#if (NET452 || NETSTANDARD2_0)
        protected internal virtual async Task InsertAsync(object entity, EntityMetadata entityMetadata)
        {
            if (entity == null) throw new ArgumentNullException("entity");


            if (IsAutomaticAuditDateFieldsEnabled)
            {
                TrySetAuditDate(this.SpecialFieldNames.CreatedDateFieldName, entity, entityMetadata);
                TrySetAuditDate(this.SpecialFieldNames.ModifiedDateFieldName, entity, entityMetadata);
            }
            if (IsAutomaticAuditUserFieldsEnabled)
            {
                if (this.CurrentUserIdAsyncGetter == null)
                {
                    TrySetAuditUser(this.SpecialFieldNames.CreatedByFieldName, entity, entityMetadata);
                    TrySetAuditUser(this.SpecialFieldNames.ModifiedByFieldName, entity, entityMetadata);
                }
                else
                {
                    await TrySetAuditUserAsync(this.SpecialFieldNames.CreatedByFieldName, entity, entityMetadata).ConfigureAwait(false);
                    await TrySetAuditUserAsync(this.SpecialFieldNames.ModifiedByFieldName, entity, entityMetadata).ConfigureAwait(false);
                }
            }

            DbCommand cmd = null;

            var cmdExecutor = new CommandExecutor(this, false)
            {
                GetCommandFunc = () => cmd = this.commandBuilder.GetInsertCommand(entity, entityMetadata)
            };

            object autogeneratedFieldValue = null;
            string autogeneratedFieldName = entityMetadata.SequenceFieldName;
            if (entityMetadata.AutoIncrementFieldName != null) autogeneratedFieldName = entityMetadata.AutoIncrementFieldName;
            if (autogeneratedFieldName == null)
            {
                await cmdExecutor.ExecuteNonQueryAsync().ConfigureAwait(false);
                if (entityMetadata.IsPrimaryKeyGuid)
                {
                    Guid primaryKeyValue = (Guid)entity.GetPropertyValue(entityMetadata.PrimaryKeyPropertyName);
                    if (primaryKeyValue == Guid.Empty)
                    {
                        entity.SetPropertyValue(entityMetadata.PrimaryKeyPropertyName, cmd.Parameters[EntityLiteProvider.ParameterPrefix + entityMetadata.PrimaryKeyPropertyName].Value);
                    }
                }

            }
            else
            {
                if (EntityLiteProvider.AutoGeneratedFieldFetchMode == AutoGeneratedFieldFetchMode.OutputParameter)
                {
                    cmdExecutor.SetOutPutParametersAction = (dbcmd) =>
                    {
                        var autogeneratedFieldParam = dbcmd.Parameters.Cast<IDataParameter>().FirstOrDefault(x => x.Direction == ParameterDirection.Output);
                        if (autogeneratedFieldParam == null)
                        {
                            throw new InvalidOperationException(string.Format("There is no output parameter in insert command for autogenerated field {0}.{1},", entityMetadata.EntityType.Name, autogeneratedFieldName));
                        }
                        autogeneratedFieldValue = autogeneratedFieldParam.Value;
                    };
                    await cmdExecutor.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                else
                {
                    autogeneratedFieldValue = await cmdExecutor.ExecuteScalarAsync().ConfigureAwait(false);
                }
                Type autogeneratedFieldType = autogeneratedFieldValue.GetType();
                if (autogeneratedFieldType == entityMetadata.PrimaryKeyType)
                {
                    entityMetadata.Setters[autogeneratedFieldName](entity, autogeneratedFieldValue);
                }
                else
                {
                    entityMetadata.Setters[autogeneratedFieldName](entity, Convert.ChangeType(autogeneratedFieldValue, entityMetadata.PrimaryKeyType));
                }
            }
            if (entityMetadata.Properties.ContainsKey(SpecialFieldNames.EntityRowVersionFieldName))
            {
                entityMetadata.Setters[SpecialFieldNames.EntityRowVersionFieldName](entity, Convert.ChangeType(1, entityMetadata.Properties[SpecialFieldNames.EntityRowVersionFieldName].PropertyInfo.PropertyType));
            }
        }
#endif

        protected internal bool Update(object entity)
        {
            return Update(entity, GetValidatedForUpdateSortedFields(entity));
        }

#if (NET452 || NETSTANDARD2_0)
        protected internal Task<bool> UpdateAsync(object entity)
        {
            return UpdateAsync(entity, GetValidatedForUpdateSortedFields(entity));
        }
#endif


        protected internal bool Update(object entity, params string[] fieldsToUpdate)
        {
            return Update(entity, GetValidatedForUpdateSortedFields(entity, fieldsToUpdate));
        }

#if (NET452 || NETSTANDARD2_0)        
        protected internal Task<bool> UpdateAsync(object entity, params string[] fieldsToUpdate)
        {
            return UpdateAsync(entity, GetValidatedForUpdateSortedFields(entity, fieldsToUpdate));
        }
#endif
        private object GeByPrimaryKeyIncludingJustPkAndRowVersionFields(Type entityType, object entity)
        {
            IQueryLite q = GetByPrimaryKeyIncludingJustPkAndRowVersionFieldsQuery(entityType, entity);
            return q.FirstOrDefault();
        }

#if (NET452 || NETSTANDARD2_0)
        private Task<object> GeByPrimaryKeyIncludingJustPkAndRowVersionFieldsAsync(Type entityType, object entity)
        {
            IQueryLite q = GetByPrimaryKeyIncludingJustPkAndRowVersionFieldsQuery(entityType, entity);
            return q.FirstOrDefaultAsync();
        }
#endif

        private IQueryLite GetByPrimaryKeyIncludingJustPkAndRowVersionFieldsQuery(Type entityType, object entity)
        {
            var q = this.CreateQueryLite(entityType, Projection.BaseTable);
            var metadata = entityType.GetEntityMetadata();
            foreach (var pkf in metadata.PrimaryKeyPropertyNames)
            {
                var prop = metadata.Properties[pkf];
                q.Filter.Add(new ConditionLite
                {
                    FieldName = pkf,
                    LogicalOperator = LogicalOperatorLite.And,
                    Operator = OperatorLite.Equals,
                    FieldValue = entity.GetPropertyValue(pkf)
                });
                q.FieldList.Add(pkf);
            }
            if (metadata.Properties.ContainsKey(this.SpecialFieldNames.EntityRowVersionFieldName))
            {
                q.FieldList.Add(SpecialFieldNames.EntityRowVersionFieldName);
            }
            return q;
        }

        protected internal virtual bool Update(object entity, List<string> sortedFields)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();
            var metadata = entityType.GetEntityMetadata();

            SetAuditObjectResult previousModifiedBy = default(SetAuditObjectResult);
            SetAuditObjectResult previousModifiedDate = default(SetAuditObjectResult);

            if (IsAutomaticAuditUserFieldsEnabled)
            {
                previousModifiedBy = TrySetAuditUser(this.SpecialFieldNames.ModifiedByFieldName, entity, metadata);
            }
            if (IsAutomaticAuditDateFieldsEnabled)
            {
                previousModifiedDate = TrySetAuditDate(this.SpecialFieldNames.ModifiedDateFieldName, entity, metadata);
            }
            var cmd = new CommandExecutor(this, false)
            {
                GetCommandFunc = () => this.commandBuilder.GetUpdateCommand(entity, sortedFields, metadata)
            };
            var affectedRecords = cmd.ExecuteNonQuery();
            var identity = entity.TryGetId();
            if (identity != null) IdentityMap.Remove(entityType, identity);
            bool hasEntityRowVersion = metadata.Properties.ContainsKey(this.SpecialFieldNames.EntityRowVersionFieldName);
            object freshEntity = hasEntityRowVersion ?  GeByPrimaryKeyIncludingJustPkAndRowVersionFields(entityType, entity) : null;
            try
            {
                if (affectedRecords == 0)
                {
                    if (previousModifiedBy.IsSet) entity.SetPropertyValue(SpecialFieldNames.ModifiedByFieldName, previousModifiedBy.PreviousValue);
                    if (previousModifiedDate.IsSet) entity.SetPropertyValue(SpecialFieldNames.ModifiedDateFieldName, previousModifiedDate.PreviousValue);

                    if (!hasEntityRowVersion) freshEntity = GeByPrimaryKeyIncludingJustPkAndRowVersionFields(entityType, entity);
                    if (freshEntity == null) throw new RowNotFoundException("Attempt to update an inexistent row");
                    if (hasEntityRowVersion && freshEntity != null &&
                        !object.Equals(entity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName), 
                        freshEntity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName)))
                    {
                        throw new DBConcurrencyException("Concurrency conflict detected. The row has been modified after it was read");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            finally
            {
                if (freshEntity != null && hasEntityRowVersion)
                {
                    entity.SetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName, freshEntity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName));
                }
            }
            return affectedRecords > 0;
        }

#if (NET452 || NETSTANDARD2_0)
        protected internal virtual async Task<bool> UpdateAsync(object entity, List<string> sortedFields)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();
            var metadata = entityType.GetEntityMetadata();
            SetAuditObjectResult previousModifiedBy = default(SetAuditObjectResult);
            SetAuditObjectResult previousModifiedDate = default(SetAuditObjectResult);

            if (IsAutomaticAuditUserFieldsEnabled)
            {
                if (this.CurrentUserIdAsyncGetter == null)
                {
                     previousModifiedBy = TrySetAuditUser(this.SpecialFieldNames.ModifiedByFieldName, entity, metadata);
                }
                else
                {
                    previousModifiedBy = await TrySetAuditUserAsync(this.SpecialFieldNames.ModifiedByFieldName, entity, metadata).ConfigureAwait(false);
                }
            }
            if (IsAutomaticAuditDateFieldsEnabled)
            {
                previousModifiedDate = TrySetAuditDate(this.SpecialFieldNames.ModifiedDateFieldName, entity, metadata);
            }
            var cmd = new CommandExecutor(this, false)
            {
                GetCommandFunc = () => this.commandBuilder.GetUpdateCommand(entity, sortedFields, metadata)
            };
            var affectedRecords = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            var identity = entity.TryGetId();
            if (identity != null) IdentityMap.Remove(entityType, identity);
            bool hasEntityRowVersion = metadata.Properties.ContainsKey(this.SpecialFieldNames.EntityRowVersionFieldName);
            object freshEntity = hasEntityRowVersion ? await GeByPrimaryKeyIncludingJustPkAndRowVersionFieldsAsync(entityType, entity).ConfigureAwait(false) : null;
            try
            {
                if (affectedRecords == 0)
                {
                    if (previousModifiedBy.IsSet) entity.SetPropertyValue(SpecialFieldNames.ModifiedByFieldName, previousModifiedBy.PreviousValue);
                    if (previousModifiedDate.IsSet) entity.SetPropertyValue(SpecialFieldNames.ModifiedDateFieldName, previousModifiedDate.PreviousValue);

                    if (!hasEntityRowVersion) freshEntity = await GeByPrimaryKeyIncludingJustPkAndRowVersionFieldsAsync(entityType, entity).ConfigureAwait(false);
                    if (freshEntity == null) throw new RowNotFoundException("Attempt to update an inexistent row");
                    if (hasEntityRowVersion && freshEntity != null &&
                        !object.Equals(entity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName),
                        freshEntity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName)))
                    {
                        throw new DBConcurrencyException("Concurrency conflict detected. The row has been modified after it was read");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            finally
            {
                if (freshEntity != null && hasEntityRowVersion)
                {
                    entity.SetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName, freshEntity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName));
                }
            }
            return affectedRecords > 0;
        }
#endif

        protected internal List<string> GetValidatedForUpdateSortedFields(object entity, string[] fieldsToUpdate = null)
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
            sortedFields.EnsureField(metadata, SpecialFieldNames.DbChangeNumberFieldName);

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
