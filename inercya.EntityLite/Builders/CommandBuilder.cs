using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using NLog;
using System.Data.SqlTypes;
using System.Globalization;
using inercya.EntityLite.Extensions;
using inercya.EntityLite.Collections;

namespace inercya.EntityLite.Builders
{
	public enum CommandKind
	{

		Insert,
		Update,
		DeleteCommand
	}


	internal class CommandBuilder : IDisposable
	{
		private Logger Log = NLog.LogManager.GetLogger(typeof(DataService).FullName);

		private readonly DataService dataAccess;

		public CommandBuilder(DataService dataAccess)
		{
			this.dataAccess = dataAccess;
		}



        private Dictionary<Type, string, DbCommand> updateCommandCache = new Dictionary<Type, string, DbCommand>();

        private Dictionary<Type, DbCommand> insertCommandCache = new Dictionary<Type, DbCommand>();

        private Dictionary<Type, DbCommand> deleteCommandCache = new Dictionary<Type, DbCommand>();


        public DbCommand GetUpdateCommand(object entity, IEnumerable<string> fieldsToUpdate)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();
            string fieldsKey = fieldsToUpdate.Join();
            DbCommand command;
            if (updateCommandCache.TryGetValue(entityType, fieldsKey, out command))
            {
                SetParameterValuesFromObject(command, entity);
            }
            else
            {
                command = GenerateUpdateCommand(entity, fieldsToUpdate);
                updateCommandCache.Add(entityType, fieldsKey, command);
            }
            return command;
        }

		public  DbCommand GetInsertCommand(object entity)
		{
			Type entityType = entity.GetType();
			DbCommand command;
			if (insertCommandCache.TryGetValue(entityType, out command))
			{
				SetParameterValuesFromObject(command, entity);
			}
			else
            {
				command = GenerateInsertCommand(entity);
                insertCommandCache.Add(entityType, command);
			}
			return command;
		}

        public DbCommand GetDeleteCommand(object entity)
        {
            Type entityType = entity.GetType();
            DbCommand command;
            if (deleteCommandCache.TryGetValue(entityType, out command))
            {
                SetParameterValuesFromObject(command, entity);
            }
            else
            {
                command = GenerateDeleteCommand(entity);
                deleteCommandCache.Add(entityType, command);
            }
            return command;
        }

        private DbCommand GenerateUpdateCommand(object entity, IEnumerable<string> fieldsToUpdate)
        {
            if (entity == null) throw new ArgumentNullException("entity", "to generate a update command, entity argument must be not null ");

            Type entityType = entity.GetType();
            EntityMetadata entityMetadata = entityType.GetEntityMetadata();
            string baseTable = entityMetadata.BaseTableName;
            if (string.IsNullOrEmpty(baseTable))
            {
                throw new InvalidOperationException("cannot generate update command for entity " + entityType.Name + " because it does not have a base table");
            }
            if (entityMetadata.PrimaryKeyPropertyNames.Count == 0)
            {
                throw new InvalidOperationException("cannot generate update command for entity " + entityType.Name + " because it does not have a primary key");
            }

            string fullTableName = entityMetadata.GetFullTableName(this.dataAccess.DefaultSchemaName);

            IPropertyGetterDictionary getters = PropertyHelper.GetPropertyGetters(entityType);
            DbCommand cmd = dataAccess.Connection.CreateCommand();
            StringBuilder commandText = new StringBuilder();
            commandText.Append("\nUPDATE ").Append(fullTableName);
            bool firstTime = true;
            bool hasEntityRowVersionField = false;
            foreach (var propertyName in fieldsToUpdate)
            {
                PropertyMetadata propMetadata = null;
                if (!entityMetadata.UpdatableProperties.TryGetValue(propertyName, out propMetadata)) continue;
                SqlFieldAttribute field = propMetadata.SqlField;
                if (!field.IsKey
                    && !string.Equals(field.BaseColumnName, dataAccess.SpecialFieldNames.CreatedByFieldName, StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(field.BaseColumnName, dataAccess.SpecialFieldNames.CreatedDateFieldName, StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    if (firstTime)
                    {
                        commandText.Append(" SET\n\t");
                        firstTime = false;
                    }
                    else
                    {
                        commandText.Append(",\n\t");
                    }
                    if (string.Equals(propertyName, dataAccess.SpecialFieldNames.EntityRowVersionFieldName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        hasEntityRowVersionField = true;
                        commandText.Append(dataAccess.SpecialFieldNames.EntityRowVersionFieldName).Append(" = ").Append(dataAccess.SpecialFieldNames.EntityRowVersionFieldName).Append(" + 1");
                    }
                    else
                    {
                        string parameterName = dataAccess.ParameterPrefix + propertyName;
                        commandText.Append(field.BaseColumnName).Append(" = ").Append(parameterName);
                        IDbDataParameter param = CreateParameter(propMetadata, propertyName);
                        object fieldValue = getters[propertyName](entity);
                        SetValueToCommandParameter(entity, getters, propertyName, param);
                        cmd.Parameters.Add(param);
                    }
                }
            }

            firstTime = true;

            IEnumerable<string> whereFields = entityMetadata.PrimaryKeyPropertyNames;
            if (hasEntityRowVersionField) whereFields = whereFields.Concat(new string[] { dataAccess.SpecialFieldNames.EntityRowVersionFieldName });
            foreach (string whereField in whereFields)
            {
                var property = entityMetadata.Properties[whereField];
                SqlFieldAttribute field = property.SqlField;
                if (string.Equals(whereField, field.BaseColumnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    string parameterName = dataAccess.ParameterPrefix + whereField;
                    if (firstTime)
                    {
                        commandText.Append("\nWHERE\n\t");
                        firstTime = false;
                    }
                    else
                    {
                        commandText.Append("\n\tAND ");
                    }
                    commandText.Append(field.BaseColumnName).Append(" = ").Append(parameterName);

                    IDbDataParameter parameter = CreateParameter(property, whereField);
                    PropertyGetter getter;
                    if (getters.TryGetValue(whereField, out getter))
                    {
                        object parameterValue = getter(entity);
                        parameter.Value = parameterValue == null ? DBNull.Value : parameterValue;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }

            cmd.CommandText = commandText.ToString();
            return cmd;
        }


		private DbCommand GenerateInsertCommand(object entity)
		{
			Type entityType = entity.GetType();
			EntityMetadata entityMetadata = entityType.GetEntityMetadata();
			if (entityMetadata == null)
			{
				throw new InvalidOperationException("cannot generate insert command for entity " + entityType.Name + " because it does not have metadata");
			}
			string baseTable = entityMetadata.BaseTableName;
			if (string.IsNullOrEmpty(baseTable))
			{
				throw new InvalidOperationException("cannot generate insert command for entity " + entityType.Name + " because it does not have a base table");
			}

            string fullTableName = entityMetadata.GetFullTableName(this.dataAccess.DefaultSchemaName);
			IPropertyGetterDictionary getters = PropertyHelper.GetPropertyGetters(entityType);

			DbCommand cmd = dataAccess.Connection.CreateCommand();
			StringBuilder commandText = new StringBuilder();
			StringBuilder valuesText = new StringBuilder();
            commandText.Append("\nINSERT INTO  ").Append(fullTableName);
			bool firstTime = true;
			foreach (var kv in entityMetadata.UpdatableProperties)
			{
				SqlFieldAttribute field = kv.Value.SqlField;
				string propertyName = kv.Key;
				if (firstTime)
				{
					commandText.Append("(");
					valuesText.Append("\nVALUES(");
					firstTime = false;
				}
				else
				{
					commandText.Append(", ");
					valuesText.Append(", ");
				}
				commandText.Append(field.BaseColumnName);
				if (string.Equals(propertyName, dataAccess.SpecialFieldNames.EntityRowVersionFieldName, StringComparison.InvariantCultureIgnoreCase))
				{
					valuesText.Append("1");
				}
				else
				{
					string parameterName = dataAccess.ParameterPrefix + propertyName;
					valuesText.Append(parameterName);
					IDbDataParameter param = CreateParameter(kv.Value, propertyName);
					SetValueToCommandParameter(entity, getters, propertyName, param);
					cmd.Parameters.Add(param);
				}
				
			}
			commandText.Append(")");
			valuesText.Append(")");
			commandText.Append(valuesText.ToString()).Append(";");

			if (!string.IsNullOrEmpty(entityMetadata.AutoIncrementFieldName))
			{
				switch (dataAccess.Provider)
				{
					case Provider.SqlClient:
						commandText.Append("\nSELECT SCOPE_IDENTITY() AS ").Append(entityMetadata.AutoIncrementFieldName).Append(";");
						break;
					case Provider.SQLite:
						commandText.Append("\nSELECT last_insert_rowid() AS ").Append(entityMetadata.AutoIncrementFieldName).Append(";");
						break;
					case Provider.OracleClient:
						throw new NotImplementedException("autoincrement fields for oracle are not implemented yet");
					default:
						throw new NotSupportedException("Provider " + dataAccess.Provider.ToString() + "  not supported");
				}
			}
			cmd.CommandText = commandText.ToString();
			return cmd;
		}

		private static void SetValueToCommandParameter(object entity, IPropertyGetterDictionary getters, string propertyName, IDbDataParameter param)
		{
			var getter = getters[propertyName];
			if (getter == null) return;
			object fieldValue = getter(entity);
			var nullable = fieldValue as INullable;
			if (nullable != null && nullable.IsNull)
			{
				param.Value = DBNull.Value;
			}
			else
			{
				if (fieldValue != null && (param.DbType == DbType.String || param.DbType == DbType.AnsiString) && !(fieldValue is string))
				{
					var convertible = fieldValue as IConvertible;
					if (convertible != null)
					{
						fieldValue = convertible.ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						fieldValue = fieldValue.ToString();
					}
				}
				param.Value = fieldValue ?? DBNull.Value;
			}
		}



		private void SetParameterValuesFromObject(DbCommand command, object obj)
		{
			Type type = obj.GetType();
			IPropertyGetterDictionary getters = PropertyHelper.GetPropertyGetters(type);
			foreach (DbParameter param in command.Parameters)
			{
				if ((param.Direction & ParameterDirection.Input) == ParameterDirection.Input)
				{
					SetValueToCommandParameter(obj, getters, param.SourceColumn, param);
				}
			}
		}

		protected IDbDataParameter CreateParameter(PropertyMetadata property, string fieldName)
		{
			var field = property.SqlField;
			string parameterName = dataAccess.ParameterPrefix + fieldName;
			IDbDataParameter parameter = dataAccess.ProviderFactory.CreateParameter();
			parameter.ParameterName = parameterName;
			Type propertyType = property.PropertyInfo.PropertyType.UndelyingType();
			if (propertyType.FullName.StartsWith("Microsoft.SqlServer.Types.Sql"))
			{
				parameter.DbType = DbType.String;
				if (propertyType.Name.StartsWith("SqlHierarchyId"))
				{
					parameter.Size = 4000;
				}
				else
				{
					parameter.Size = 1073741823;
				}
				return parameter;
			}
			parameter.DbType = field.DbType;
			parameter.Size = field.Size;
			parameter.SourceColumn = fieldName;
			if (field.Precision != 255 && field.Precision != 0)
			{
				parameter.Precision = field.Precision;
			}
			if (field.Scale != 255)
			{
				parameter.Scale = field.Scale;
			}
			return parameter;
		}

		private DbCommand GenerateDeleteCommand(object entity)
		{
			try
			{
				if (entity == null) throw new ArgumentNullException("entity");
				Type entityType = entity.GetType();
				EntityMetadata entityMetadata = entityType.GetEntityMetadata();
				if (entityMetadata == null)
				{
					throw new InvalidOperationException("cannot generate delete command for entity " + entityType.Name + " because it does not have metadata");
				}
				string baseTable = entityMetadata.BaseTableName;
				if (string.IsNullOrEmpty(baseTable))
				{
					throw new InvalidOperationException("cannot generate delete command for entity " + entityType.Name + " because it does not have a base table");
				}
				if (entityMetadata.PrimaryKeyPropertyNames.Count == 0)
				{
					throw new InvalidOperationException("cannot generate delete command for entity " + entityType.Name + " because it does not have a primary key");
				}
				IPropertyGetterDictionary getters = PropertyHelper.GetPropertyGetters(entityType);
				DbCommand cmd = dataAccess.Connection.CreateCommand();
				StringBuilder commandText = new StringBuilder();
                string fullTableName = entityMetadata.GetFullTableName(this.dataAccess.DefaultSchemaName);
				commandText.Append("\nDELETE FROM ").Append(fullTableName);
				bool firstTime = true;
				foreach (string fieldName in entityMetadata.PrimaryKeyPropertyNames)
				{
					var property = entityMetadata.Properties[fieldName];
					SqlFieldAttribute field = property.SqlField;
					string parameterName = dataAccess.ParameterPrefix + fieldName;
					if (firstTime)
					{
						commandText.Append("\nWHERE\n\t");
						firstTime = false;
					}
					else
					{
						commandText.Append("\n\tAND ");
					}
					commandText.Append(field.BaseColumnName).Append(" = ").Append(parameterName);

					IDbDataParameter param = CreateParameter(property, fieldName);
					SetValueToCommandParameter(entity, getters, fieldName, param);
					cmd.Parameters.Add(param);
				}
				cmd.CommandText = commandText.ToString();
				return cmd;
			}
			catch (Exception ex)
			{
				if (entity == null)
				{
					Log.ErrorException("Error generating delete command", ex);
				}
				else
				{
					Log.ErrorException(string.Format("Error generating delete command for entity of type {0} with primary key: {1}", entity.GetType().Name, entity.GetPrimaryKey().ToListString() ?? "{no id}"), ex);
				}
				throw;
			}

		}

		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				IsDisposed = true;
				foreach (DbCommand command in this.insertCommandCache.Values)
				{
					command.Dispose();
				}
                this.insertCommandCache.Clear();

                foreach (DbCommand command in this.deleteCommandCache.Values)
                {
                    command.Dispose();
                }
                this.deleteCommandCache.Clear();

                foreach (DbCommand command in this.updateCommandCache.Values)
                {
                    command.Dispose();
                }
                this.updateCommandCache.Clear();
			}
		}
	}
}
