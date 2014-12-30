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
using inercya.EntityLite.Providers;

namespace inercya.EntityLite
{

    public partial class DataService : IDisposable
    {

        public static readonly IDictionary<string, Func<DataService, IEntityLiteProvider>> EntityLiteProviderFactories = new Dictionary<string, Func<DataService, IEntityLiteProvider>>();

        private static readonly CacheLite<string, ConnectionStringSettings> connectionStrings = new CacheLite<string, ConnectionStringSettings>((name) => ConfigurationManager.ConnectionStrings[name]);

        public object CurrentUserId { get; set; }

		public int MaxRetries { get; protected set; }
		public int InitialMillisecondsRetryDelay { get; protected set; }
		public SpecialFieldNames SpecialFieldNames { get; set; }

        private IEntityLiteProvider _entityLiteProvider;

        public int CommandTimeout { get; set; }

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

		private static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();

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

        private static CacheLite<string, DbProviderFactory> providerFactoriesCache;

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
            providerFactoriesCache = new CacheLite<string, DbProviderFactory>(providerName => DbProviderFactories.GetFactory(providerName));
            EntityLiteProviderFactories.Add(SqlServerEntityLiteProvider.ProviderName, (ds) => new SqlServerEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add(SqliteEntityLiteProvider.ProviderName, (ds) => new SqliteEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add(MySqlEntityLiteProvider.ProviderName, (ds) => new MySqlEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add(OracleEntityLiteProvider.ProviderName, (ds) => new OracleEntityLiteProvider(ds));
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
                        throw new InvalidOperationException("The ProviderName property of the data service has not been set. Please check that the connection string \"" + this.ConnectionStringName + "\" in the configuration file exists and includes the attribute ProviderName");
                    }
                    _dbProviderFactory = providerFactoriesCache.GetItem(this.ProviderName);
                }
                return _dbProviderFactory;
            }
        }

        private DbConnection _connection;
        public DbConnection Connection 
        {
            get
            {
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

        public virtual void Commit()
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

        public virtual void Rollback()
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

        private bool SetAuditDate(string fieldName, object entity, out object previousValue)
        {
            Type entityType = entity.GetType();
            return SetAuditDate(fieldName, entity, EntityMetadata.GetEntityMetadata(entityType), out previousValue);
        }

        private bool SetAuditDate(string fieldName, object entity, EntityMetadata metadata, out object previousValue)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            PropertyGetter getter;
            if (metadata.Getters.TryGetValue(fieldName, out getter))
            {
                previousValue = getter(entity);
            }
            else
            {
                previousValue = null;
            }
            PropertySetter setter = null;
            if (!string.IsNullOrEmpty(fieldName) && metadata.Setters.TryGetValue(fieldName, out setter))
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
                else if (type == typeof(string))
                {
                    setter(entity, DateTimeOffset.Now.ToString("O"));
                }
                else
                {
                    throw new NotSupportedException("The field \"" + fieldName + "\" is of an unsuported type " + type.Name);
                }
                return true;
            }
            return false;
        }

        private bool SetAuditUser(string fieldName, object entity, out object previousValue)
        {
            Type entityType = entity.GetType();
            return SetAuditUser(fieldName, entity, EntityMetadata.GetEntityMetadata(entityType), out previousValue);
        }

        private bool SetAuditUser(string fieldName, object entity, EntityMetadata entityMetadata, out object previousValue)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            var entityType = entityMetadata.EntityType;
            var getters = entityMetadata.Getters;
            PropertyGetter getter;
            if (getters.TryGetValue(fieldName, out getter))
            {
                previousValue = getter(fieldName);
            }
            else
            {
                previousValue = null;
            }
            if (this.CurrentUserId == null) return false;
            var setters = PropertyHelper.GetPropertySetters(entityType);
            PropertySetter setter = null;
            if (!string.IsNullOrEmpty(fieldName) && setters.TryGetValue(fieldName, out setter))
            {
                setter(entity, this.CurrentUserId);
                return true;
            }
            return false;
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

		protected internal virtual void Insert(object entity, EntityMetadata entityMetadata)
		{
		    if (entity == null) throw new ArgumentNullException("entity");

            object previousValue = null;

            if (IsAutomaticAuditDateFieldsEnabled)
            {
                SetAuditDate(this.SpecialFieldNames.CreatedDateFieldName, entity, entityMetadata, out previousValue);
                SetAuditDate(this.SpecialFieldNames.ModifiedDateFieldName, entity, entityMetadata, out previousValue);
            }
            if (IsAutomaticAuditUserFieldsEnabled)
            {
                SetAuditUser(this.SpecialFieldNames.CreatedByFieldName, entity, entityMetadata, out previousValue);
                SetAuditUser(this.SpecialFieldNames.ModifiedByFieldName, entity, entityMetadata, out previousValue);
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
                return;
            }
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

        protected internal int Update(object entity)
        {
            return Update(entity, GetValidatedForUpdateSortedFields(entity));
        }

        protected internal int Update(object entity, params string[] fieldsToUpdate)
        {
            return Update(entity, GetValidatedForUpdateSortedFields(entity, fieldsToUpdate));
        }

        private object GeByPrimaryKeyIncludingJustPkAndRowVersionFields(Type entityType, object entity)
        {
            var q = this.CreateQueryLite(entityType, Projection.BaseTable);
            var metadata = entityType.GetEntityMetadata();
            foreach (var pkf in metadata.PrimaryKeyPropertyNames )
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
            return q.FirstOrDefault();
        }

        protected internal virtual int Update(object entity, List<string> sortedFields)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();
            var metadata = entityType.GetEntityMetadata();
            bool isModifiedBySet = false;
            bool isModifiedDateSet = false;
            object previousModifiedBy = null;
            object previousModifiedDate = null;

            if (IsAutomaticAuditUserFieldsEnabled)
            {
                isModifiedBySet = SetAuditUser(this.SpecialFieldNames.ModifiedByFieldName, entity, out previousModifiedBy);
            }
            if (IsAutomaticAuditDateFieldsEnabled)
            {
                isModifiedDateSet = SetAuditDate(this.SpecialFieldNames.ModifiedDateFieldName, entity, out previousModifiedDate);
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
                    if (isModifiedBySet) entity.SetPropertyValue(SpecialFieldNames.ModifiedByFieldName, previousModifiedBy);
                    if (isModifiedDateSet) entity.SetPropertyValue(SpecialFieldNames.ModifiedDateFieldName, previousModifiedDate);

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
                        return 0;
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
            return affectedRecords;
        }

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
