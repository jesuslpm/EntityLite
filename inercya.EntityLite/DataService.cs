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
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace inercya.EntityLite
{

    public partial class DataService : IDisposable
    {

        public static readonly IDictionary<string, Func<DataService, IEntityLiteProvider>> EntityLiteProviderFactories = new Dictionary<string, Func<DataService, IEntityLiteProvider>>();


        private object _currentUserId;

        [Obsolete("Use CurrentUserId property instead")]
        public Func<object> CurrentUserIdGetter { get; set; }

        [Obsolete("Use CurrentUserId property instead")]
        public Func<Task<object>> CurrentUserIdAsyncGetter { get; set; }

        public object CurrentUserId 
        {
#pragma warning disable CS0618 // Type or member is obsolete
            get 
            {

                if (CurrentUserIdGetter != null) return CurrentUserIdGetter();
                if (CurrentUserIdAsyncGetter != null) return CurrentUserIdAsyncGetter().GetAwaiter().GetResult();
                return _currentUserId;
            }
            set
            {
                _currentUserId = value;
                CurrentUserIdGetter = null;
                CurrentUserIdAsyncGetter = null;
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }



        public async Task<object> GetCurrentUserIdAsync()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (CurrentUserIdAsyncGetter != null) return await CurrentUserIdAsyncGetter();
#pragma warning restore CS0618 // Type or member is obsolete
            return CurrentUserId;
        }

        public Func<string> ApplicationContextGetter { get; set; }

        public SortedDictionary<string, string> AppContext { get; } = new();

		public int MaxRetries { get; set; }
		public int InitialMillisecondsRetryDelay { get; protected set; }
		public SpecialFieldNames SpecialFieldNames { get; set; }

        public int CommandTimeout { get; set; }

        public DateTimeKind AuditDateTimeKind { get; set; }
        public string SequencePrefix { get; set; }
        public string SequenceSuffix { get; set; }
        public string ViewPrefix { get; set; }

        public bool IsPreventingSuperfluousUpdatesEnabled { get; set; } = true;

        public IEntityLiteProvider EntityLiteProvider { get; set; }


        public TextTransform EntityNameToEntityViewTransform { get; set; }

		private CommandBuilder commandBuilder;

        private static ILogger logger;

        public Guid InstanceId { get; private set; } = Guid.NewGuid();
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
                        logger = ConfigurationLite.LoggerFactory.CreateLogger<DataService>();
                    }
                    catch { }
                }
                return logger;
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
#pragma warning disable CA1713 // Events should not have 'Before' or 'After' prefix
        public event EventHandler BeforeCommit;
        public event EventHandler AfterCommit;
        public event EventHandler BeforeRollback;
        public event EventHandler AfterRollback;
#pragma warning restore CA1713 // Events should not have 'Before' or 'After' prefix

        public string ConnectionString { get; set; }

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
            EntityLiteProviderFactories.Add("System.Data.SqlClient.SqlClientFactory", (ds) => new SqlServerEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add("Microsoft.Data.SqlClient.SqlClientFactory", (ds) => new SqlServerEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add("System.Data.SQLite.SQLiteFactory", (ds) => new SqliteEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add("MySql.Data.MySqlClient.MySqlClientFactory", (ds) => new MySqlEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add("Oracle.ManagedDataAccess.Client.OracleClientFactory", (ds) => new OracleEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add(DevArtEntityLiteProvider.ProviderName, (ds) => new DevArtEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add("Npgsql.NpgsqlFactory", (ds) => new NpgsqlEntityLiteProvider(ds));
            EntityLiteProviderFactories.Add("FirebirdSql.Data.FirebirdClient.FirebirdClientFactory", (ds) => new FirebirdEntityLiteProvider(ds));
        }

        private DbProviderFactory _dbProviderFactory;
        public DbProviderFactory DbProviderFactory
        {
            get => _dbProviderFactory;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                var name = value.GetType().FullName;
                if (!EntityLiteProviderFactories.TryGetValue(name, out var factory))
                {
                    throw new InvalidOperationException($"The db provider factory {name} is not supported");
                }
                EntityLiteProvider = factory(this);
                _dbProviderFactory = value;
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
                        throw new InvalidOperationException("The ConnectionString property of the data service has not been set.");
                    }
                    if (DbProviderFactory == null)
                    {
                        throw new InvalidOperationException("The DbProviderFactory property of the data service has not been set.");
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
            if (this.Connection.State == ConnectionState.Broken)
            {
                try { _connection.Dispose(); } catch { }
                this._transaction = null;
                this._connection = null;
                this.TransactionCount = 0;
                throw new ConnectionBrokenException("The connection is broken and cannot be used.");
			}

            if (this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Open();
            }
        }

        private static Task completedTask = Task.FromResult((object)null);

        public virtual async Task OpenConnectionAsync()
        {
			if (this.Connection.State == ConnectionState.Broken)
			{
				try { _connection.Dispose(); } catch { }
				this._transaction = null;
				this._connection = null;
				this.TransactionCount = 0;
				throw new ConnectionBrokenException("The connection is broken and cannot be used.");
			}

			if (this.Connection.State != ConnectionState.Open)
			{
				await this.Connection.OpenAsync();
			}
		}

        public int TransactionCount { get; private set; }

        public void BeginTransaction()
        {
			OpenConnection();
			if (!IsActiveTransaction)
			{
				_transaction = this.CreateTransaction();
				TransactionCount = 1;
			}
			else
			{
				TransactionCount++;
			}
		}

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
			OpenConnection();
			if (!IsActiveTransaction)
			{
				_transaction = this.CreateTransaction(isolationLevel);
				TransactionCount = 1;
			}
			else
			{
				TransactionCount++;
			}
		}

        protected virtual DbTransaction CreateTransaction()
        {
            return this.Connection.BeginTransaction();
        }

        protected virtual DbTransaction CreateTransaction(IsolationLevel isolationLevel)
        {
            return this.Connection.BeginTransaction(isolationLevel);
        }

        public bool IsActiveTransaction
        {
            get { return this.Transaction != null && this.Transaction.Connection != null && this.Transaction.Connection.State == ConnectionState.Open; }
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
                    if (_transaction != null)
                    {
                        try { _transaction.Dispose(); } catch { }
                        _transaction = null;
                        TransactionCount = 0;
					}
					throw new NoActiveTransactionException("Cannot commit because there is no active transaction");
				}
			}
			catch (Exception ex)
			{
				Log?.LogError(ex, "Error on commit");
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
					if (_transaction != null)
					{
						try { _transaction.Dispose(); } catch { }
						_transaction = null;
						TransactionCount = 0;
					}
					throw new NoActiveTransactionException("Cannot rollback because there is no active transaction");
				}
			}
			catch (Exception ex)
			{
				Log?.LogError(ex, "Error on Rollback");
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

        public DataService(DbProviderFactory dbProviderFactory, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            if (dbProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(dbProviderFactory));
            }
            this.ConnectionString = connectionString;
            this.DbProviderFactory = dbProviderFactory;
            
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
                if (commandBuilder != null)
                {
                    commandBuilder.Dispose();
                }
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#endregion


        public virtual object GetPreviousEntity(EntityMetadata metadata, object entity)
        {
            return this.GetByPrimaryKey(metadata, entity);
        }



        public virtual object GetCurrentEntity(EntityMetadata metadata, object entity)
        {
            return entity;
        }

        public virtual Task<object> GetPreviousEntityAsync(EntityMetadata metadata, object entity)
        {
            return this.GetByPrimaryKeyAsync(metadata, entity);
        }

        public virtual Task<object> GetCurrentEntityAsync(EntityMetadata metadata, object entity)
        {
            return Task.FromResult<object>(entity);
        }

        #region Modification methods


        protected internal virtual bool Delete(object entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));
            Type entityType = entity.GetType();
            var metadata = entityType.GetEntityMetadata();

            var audit = this as IAudit;
            var isAuditableEntity = audit != null && metadata.IsAuditable;
            object previousEntity = null;

            if (isAuditableEntity)
            {
                previousEntity = this.GetPreviousEntity(metadata, entity);
                if (previousEntity == null) return false;
                BeginTransaction();
            }

            var cmd = new CommandExecutor(this, false)
            {
                GetCommandFunc = () => this.commandBuilder.GetDeleteCommand(entity, EntityMetadata.GetEntityMetadata(entityType))
            };
            var affectedRecords = cmd.ExecuteNonQuery();

			var identity = entity.TryGetId();
            if (identity != null) IdentityMap.Remove(entity.GetType(), identity);
            if (isAuditableEntity)
            {
                if (affectedRecords > 0) audit.LogChange(previousEntity, null, null, metadata);
                Commit();
            }
            return affectedRecords > 0;
		}
        protected internal virtual async Task<bool> DeleteAsync(object entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Type entityType = entity.GetType();
            var metadata = entityType.GetEntityMetadata();

            var audit = this as IAudit;
            var isAuditableEntity = audit != null && metadata.IsAuditable;
            object previousEntity = null;

            if (isAuditableEntity)
            {
                previousEntity = await this.GetPreviousEntityAsync(metadata, entity).ConfigureAwait(false);
                if (previousEntity == null) return false;
                BeginTransaction();
            }
            var cmd = new CommandExecutor(this, false)
            {
                GetCommandFunc = () => this.commandBuilder.GetDeleteCommand(entity, EntityMetadata.GetEntityMetadata(entityType))
            };
            var affectedRecords = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

            var identity = entity.TryGetId();
            if (identity != null) IdentityMap.Remove(entity.GetType(), identity);
            if (isAuditableEntity)
            {
                if (affectedRecords > 0) await audit.LogChangeAsync(previousEntity, null, null, metadata).ConfigureAwait(false);
                Commit();
            }
            return affectedRecords > 0;
        }


        internal struct SetAuditObjectResult
        {
            public bool IsSet;
            public object PreviousValue;
        }

        private SetAuditObjectResult  TrySetAuditUser(string fieldName, object entity, EntityMetadata entityMetadata)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
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

        private async Task<SetAuditObjectResult> TrySetAuditUserAsync(string fieldName, object entity, EntityMetadata entityMetadata)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
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

        private SetAuditObjectResult TrySetAuditDate(string fieldName, object entity, EntityMetadata metadata)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
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

        protected internal void Insert(object entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Type entityType = entity.GetType();
            Insert(entity, EntityMetadata.GetEntityMetadata(entityType));
        }

        protected internal virtual Task InsertAsync(object entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Type entityType = entity.GetType();
            return InsertAsync(entity, EntityMetadata.GetEntityMetadata(entityType));
        }

        protected internal virtual void Insert(object entity, EntityMetadata entityMetadata)
		{
		    if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entityMetadata == null) throw new ArgumentNullException(nameof(entityMetadata));

            if (IsAutomaticAuditDateFieldsEnabled)
            {
                TrySetAuditDate(entityMetadata.FieldPrefix + this.SpecialFieldNames.CreatedDateFieldName, entity, entityMetadata);
                TrySetAuditDate(entityMetadata.FieldPrefix + this.SpecialFieldNames.ModifiedDateFieldName, entity, entityMetadata);
            }
            if (IsAutomaticAuditUserFieldsEnabled)
            {
                TrySetAuditUser(entityMetadata.FieldPrefix + this.SpecialFieldNames.CreatedByFieldName, entity, entityMetadata);
                TrySetAuditUser(entityMetadata.FieldPrefix + this.SpecialFieldNames.ModifiedByFieldName, entity, entityMetadata);
            }

            DbCommand cmd = null;
            var audit = this as IAudit;
            bool isAuditableEntity = audit != null && entityMetadata.IsAuditable;
            if (isAuditableEntity)
            {
                this.BeginTransaction();
            }

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
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There is no output parameter in insert command for autogenerated field {0}.{1},", entityMetadata.EntityType.Name, autogeneratedFieldName));
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
                    entityMetadata.Setters[autogeneratedFieldName](entity, Convert.ChangeType(autogeneratedFieldValue, entityMetadata.PrimaryKeyType, CultureInfo.InvariantCulture));
                }
            }
            if (entityMetadata.Properties.ContainsKey(SpecialFieldNames.EntityRowVersionFieldName))
            {
                entityMetadata.Setters[SpecialFieldNames.EntityRowVersionFieldName](entity, Convert.ChangeType(1, entityMetadata.Properties[SpecialFieldNames.EntityRowVersionFieldName].PropertyInfo.PropertyType.UndelyingType(), CultureInfo.InvariantCulture));
            }
            if (isAuditableEntity)
            {
                audit.LogChange(null, this.GetCurrentEntity(entityMetadata, entity), null, entityMetadata);
                this.Commit();
            }
		}

        protected internal virtual async Task InsertAsync(object entity, EntityMetadata entityMetadata)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entityMetadata == null) throw new ArgumentNullException(nameof(entityMetadata));


            if (IsAutomaticAuditDateFieldsEnabled)
            {
                TrySetAuditDate(entityMetadata.FieldPrefix + this.SpecialFieldNames.CreatedDateFieldName, entity, entityMetadata);
                TrySetAuditDate(entityMetadata.FieldPrefix + this.SpecialFieldNames.ModifiedDateFieldName, entity, entityMetadata);
            }
            if (IsAutomaticAuditUserFieldsEnabled)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (this.CurrentUserIdAsyncGetter == null)
                {
// Call async methods when in an async method. When CurrentUserIdAsyncGetter is null TrySetAuditUerAsync fails
#pragma warning disable CA1849
                    TrySetAuditUser(entityMetadata.FieldPrefix + this.SpecialFieldNames.CreatedByFieldName, entity, entityMetadata);
                    TrySetAuditUser(entityMetadata.FieldPrefix + this.SpecialFieldNames.ModifiedByFieldName, entity, entityMetadata);
#pragma warning restore CA1849
                }
                else
                {
                    await TrySetAuditUserAsync(entityMetadata.FieldPrefix + this.SpecialFieldNames.CreatedByFieldName, entity, entityMetadata).ConfigureAwait(false);
                    await TrySetAuditUserAsync(entityMetadata.FieldPrefix + this.SpecialFieldNames.ModifiedByFieldName, entity, entityMetadata).ConfigureAwait(false);
                }
#pragma warning restore CS0618 // Type or member is obsolete
            }

            DbCommand cmd = null;
            var audit = this as IAudit;
            bool isAuditableEntity = audit != null && entityMetadata.IsAuditable;
            if (isAuditableEntity)
            {
                this.BeginTransaction();
            }
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
                            var message = string.Format(CultureInfo.InvariantCulture, 
                                "There is no output parameter in insert command for autogenerated field {0}.{1},", 
                                entityMetadata.EntityType.Name, autogeneratedFieldName);
                            throw new InvalidOperationException(message);
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
                    var value = Convert.ChangeType(autogeneratedFieldValue, entityMetadata.PrimaryKeyType, 
                        CultureInfo.InvariantCulture);
                    entityMetadata.Setters[autogeneratedFieldName](entity, value);
                }
            }
            if (entityMetadata.Properties.ContainsKey(SpecialFieldNames.EntityRowVersionFieldName))
            {
                var type = entityMetadata.Properties[SpecialFieldNames.EntityRowVersionFieldName].PropertyInfo.PropertyType;
                var value = Convert.ChangeType(1, type, CultureInfo.InvariantCulture);
                entityMetadata.Setters[SpecialFieldNames.EntityRowVersionFieldName](entity, value);
            }
            if (isAuditableEntity)
            {
                await audit.LogChangeAsync(null, await this.GetCurrentEntityAsync(entityMetadata, entity).ConfigureAwait(false), null, entityMetadata).ConfigureAwait(false);
                Commit();
            }
        }

        protected internal bool Update(object entity)
        {
            return Update(entity, GetValidatedForUpdateSortedFields(entity));
        }

        protected internal Task<bool> UpdateAsync(object entity)
        {
            return UpdateAsync(entity, GetValidatedForUpdateSortedFields(entity));
        }


        protected internal bool Update(object entity, params string[] fieldsToUpdate)
        {
            if (fieldsToUpdate != null && fieldsToUpdate.Length == 0) return false;
            return Update(entity, GetValidatedForUpdateSortedFields(entity, fieldsToUpdate));
        }

        protected internal Task<bool> UpdateAsync(object entity, params string[] fieldsToUpdate)
        {
            if (fieldsToUpdate != null && fieldsToUpdate.Length == 0) return Task.FromResult(false);
            return UpdateAsync(entity, GetValidatedForUpdateSortedFields(entity, fieldsToUpdate));
        }
        private object GeByPrimaryKeyIncludingJustPkAndRowVersionFields(EntityMetadata metadata, object entity)
        {
            IQueryLite q = GetByPrimaryKeyIncludingJustPkAndRowVersionFieldsQuery(metadata, entity);
            return q.FirstOrDefault();
        }

        private object GetByPrimaryKey(EntityMetadata metadata, object entity)
        {
            IQueryLite q = GetByPrimaryKeyQuery(metadata, entity);
            return q.FirstOrDefault();
        }

        private Task<object> GetByPrimaryKeyIncludingJustPkAndRowVersionFieldsAsync(EntityMetadata metadata, object entity)
        {
            IQueryLite q = GetByPrimaryKeyIncludingJustPkAndRowVersionFieldsQuery(metadata, entity);
            return q.FirstOrDefaultAsync();
        }

        private Task<object> GetByPrimaryKeyAsync(EntityMetadata metadata, object entity)
        {
            IQueryLite q = GetByPrimaryKeyQuery(metadata, entity);
            return q.FirstOrDefaultAsync();
        }

        private IQueryLite GetByPrimaryKeyIncludingJustPkAndRowVersionFieldsQuery(EntityMetadata metadata, object entity)
        {
            var q = this.CreateQueryLite(metadata.EntityType, Projection.BaseTable);
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

        protected IQueryLite GetByPrimaryKeyQuery(EntityMetadata metadata, object entity, string projectionName = "BaseTable" )
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            var q = this.CreateQueryLite(metadata.EntityType, projectionName);
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
            }
            return q;
        }

        protected internal virtual bool Update(object entity, List<string> sortedFields)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Type entityType = entity.GetType();
            var metadata = entityType.GetEntityMetadata();

            SetAuditObjectResult previousModifiedBy = default(SetAuditObjectResult);
            SetAuditObjectResult previousModifiedDate = default(SetAuditObjectResult);

            if (IsAutomaticAuditUserFieldsEnabled)
            {
                previousModifiedBy = TrySetAuditUser(metadata.FieldPrefix + this.SpecialFieldNames.ModifiedByFieldName, entity, metadata);
            }
            if (IsAutomaticAuditDateFieldsEnabled)
            {
                previousModifiedDate = TrySetAuditDate(metadata.FieldPrefix + this.SpecialFieldNames.ModifiedDateFieldName, entity, metadata);
            }
            var cmd = new CommandExecutor(this, false)
            {
                GetCommandFunc = () => this.commandBuilder.GetUpdateCommand(entity, sortedFields, metadata)
            };
            bool hasEntityRowVersion = metadata.Properties.ContainsKey(this.SpecialFieldNames.EntityRowVersionFieldName);
            object entityRowVersion = null;
            if (hasEntityRowVersion) entityRowVersion = entity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName);

            var audit = this as IAudit;
            var isAuditableEntity = audit != null && metadata.IsAuditable;
            object previousEntity = null;
            
            if (isAuditableEntity)
            {
                this.BeginTransaction();
                previousEntity = this.GetPreviousEntity(metadata, entity);
                if (previousEntity == null)
                {
                    throw new RowNotFoundException("Attempt to update an inexistent row");
                }
                if (hasEntityRowVersion)
                {
                    object previousEntityRowVersion = previousEntity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName);
                    if (!object.Equals(previousEntityRowVersion, entityRowVersion))
                    {
                        entity.SetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName, previousEntityRowVersion);
                        throw new DBConcurrencyException("Concurrency conflict detected. The row has been modified after it was read");
                    }
                }
            }

            var affectedRecords = cmd.ExecuteNonQuery();
            var identity = entity.TryGetId();
            if (identity != null)
            {
                IdentityMap.Remove(entityType, identity);
            }
            object freshEntity = null; 
            if (affectedRecords == 0)
            {
                freshEntity = GeByPrimaryKeyIncludingJustPkAndRowVersionFields(metadata, entity);
                if (previousModifiedBy.IsSet) entity.SetPropertyValue(SpecialFieldNames.ModifiedByFieldName, previousModifiedBy.PreviousValue);
                if (previousModifiedDate.IsSet) entity.SetPropertyValue(SpecialFieldNames.ModifiedDateFieldName, previousModifiedDate.PreviousValue);
                if (isAuditableEntity) Commit();
                if (freshEntity == null) throw new RowNotFoundException("Attempt to update an inexistent row");
                if (!hasEntityRowVersion) return false;
                object freshEntityRowVersion = freshEntity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName);
                if (!object.Equals(freshEntityRowVersion, entityRowVersion))
                {
                    entity.SetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName, freshEntityRowVersion);
                    throw new DBConcurrencyException("Concurrency conflict detected. The row has been modified after it was read");
                }
                return false;
            }
            if (hasEntityRowVersion)
            {
                var entityRowVersionFieldMetadata = metadata.Properties[SpecialFieldNames.EntityRowVersionFieldName];
                var type = entityRowVersionFieldMetadata.PropertyInfo.PropertyType;
                var incrementedEntityRowVersion = Convert.ToInt64(entityRowVersion, CultureInfo.InvariantCulture) + 1;
                var value = Convert.ChangeType(incrementedEntityRowVersion, type, CultureInfo.InvariantCulture);
                entity.SetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName, value);
            }
            if (isAuditableEntity)
            {
                audit.LogChange(previousEntity, this.GetCurrentEntity(metadata, entity), sortedFields, metadata);
                Commit();
            }
            return true;
        }

        protected internal virtual async Task<bool> UpdateAsync(object entity, List<string> sortedFields)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Type entityType = entity.GetType();
            var metadata = entityType.GetEntityMetadata();

            SetAuditObjectResult previousModifiedBy = default(SetAuditObjectResult);
            SetAuditObjectResult previousModifiedDate = default(SetAuditObjectResult);

            if (IsAutomaticAuditUserFieldsEnabled)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (this.CurrentUserIdAsyncGetter == null)
                {
// Call async methods when in an async method. when CurrentUserIdAsyncGetter is null TrySetAuditUserAsync fails
#pragma warning disable CA1849
                    previousModifiedBy = TrySetAuditUser(metadata.FieldPrefix + this.SpecialFieldNames.ModifiedByFieldName, entity, metadata);
#pragma warning restore CA1849 // Call async methods when in an async method
                }
                else
                {
                    previousModifiedBy = await TrySetAuditUserAsync(metadata.FieldPrefix + this.SpecialFieldNames.ModifiedByFieldName, entity, metadata).ConfigureAwait(false);
                }
#pragma warning restore CS0618 // Type or member is obsolete
            }
            if (IsAutomaticAuditDateFieldsEnabled)
            {
                previousModifiedDate = TrySetAuditDate(metadata.FieldPrefix + this.SpecialFieldNames.ModifiedDateFieldName, entity, metadata);
            }
            var cmd = new CommandExecutor(this, false)
            {
                GetCommandFunc = () => this.commandBuilder.GetUpdateCommand(entity, sortedFields, metadata)
            };
            bool hasEntityRowVersion = metadata.Properties.ContainsKey(this.SpecialFieldNames.EntityRowVersionFieldName);
            object entityRowVersion = null;
            if (hasEntityRowVersion) entityRowVersion = entity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName);

            var audit = this as IAudit;
            var isAuditableEntity = audit != null && metadata.IsAuditable;
            object previousEntity = null;

            if (isAuditableEntity)
            {
                this.BeginTransaction();
                previousEntity = await this.GetPreviousEntityAsync(metadata, entity).ConfigureAwait(false);
                if (previousEntity == null)
                {
                    throw new RowNotFoundException("Attempt to update an inexistent row");
                }
                if (hasEntityRowVersion)
                {
                    object previousEntityRowVersion = previousEntity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName);
                    if (!object.Equals(previousEntityRowVersion, entityRowVersion))
                    {
                        entity.SetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName, previousEntityRowVersion);
                        throw new DBConcurrencyException("Concurrency conflict detected. The row has been modified after it was read");
                    }
                }
            }

            var affectedRecords = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            var identity = entity.TryGetId();
            if (identity != null) IdentityMap.Remove(entityType, identity);
            object freshEntity = null;
            if (affectedRecords == 0)
            {
                freshEntity = await GetByPrimaryKeyIncludingJustPkAndRowVersionFieldsAsync(metadata, entity).ConfigureAwait(false);
                if (previousModifiedBy.IsSet) entity.SetPropertyValue(SpecialFieldNames.ModifiedByFieldName, previousModifiedBy.PreviousValue);
                if (previousModifiedDate.IsSet) entity.SetPropertyValue(SpecialFieldNames.ModifiedDateFieldName, previousModifiedDate.PreviousValue);
                if (isAuditableEntity) Commit();
                if (freshEntity == null) throw new RowNotFoundException("Attempt to update an inexistent row");
                if (!hasEntityRowVersion) return false;
                object freshEntityRowVersion = freshEntity.GetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName);
                if (!object.Equals(freshEntityRowVersion, entityRowVersion))
                {
                    entity.SetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName, freshEntityRowVersion);
                    throw new DBConcurrencyException("Concurrency conflict detected. The row has been modified after it was read");
                }
                return false;
            }
            if (hasEntityRowVersion)
            {
                var entityRowVersionFieldMetadata = metadata.Properties[SpecialFieldNames.EntityRowVersionFieldName];
                object value = Convert.ChangeType(Convert.ToInt64(entityRowVersion, CultureInfo.InvariantCulture) + 1, 
                    entityRowVersionFieldMetadata.PropertyInfo.PropertyType, CultureInfo.InvariantCulture);
                entity.SetPropertyValue(SpecialFieldNames.EntityRowVersionFieldName, value);
            }
            if (isAuditableEntity)
            {
                await audit.LogChangeAsync(previousEntity, await this.GetCurrentEntityAsync(metadata, entity).ConfigureAwait(false), sortedFields, metadata).ConfigureAwait(false);
                Commit();
            }
            return true;
        }

        protected internal List<string> GetValidatedForUpdateSortedFields(object entity, string[] fieldsToUpdate = null)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Type entityType = entity.GetType();
            var metadata = entityType.GetEntityMetadata();

            if (string.IsNullOrEmpty(metadata.BaseTableName))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Entity \"{0}\" is not updatable because it has no base table", entityType.Name));
            }
            List<string> sortedFields = null;

            if (fieldsToUpdate == null)
            {
                return metadata.UpdatableProperties
                    .Select(kv => kv.Key)
                    .Where(x => !metadata.PrimaryKeyPropertyNames.Contains(x, StringComparer.InvariantCultureIgnoreCase) && x != SpecialFieldNames.CreatedByFieldName && x != SpecialFieldNames.CreatedDateFieldName)
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
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Updating primary key fields is an unsupported operation. And you are trying to update the primary key field \"{0}\"", field));
                }
                if (!metadata.UpdatableProperties.ContainsKey(field) || field == SpecialFieldNames.CreatedDateFieldName || field == SpecialFieldNames.CreatedByFieldName)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is not an updatable field of table \"{1}\"", field, metadata.BaseTableName));
                }
            }
            return sortedFields;
        }

#endregion

	}
}
