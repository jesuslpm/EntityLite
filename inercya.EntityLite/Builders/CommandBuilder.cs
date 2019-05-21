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
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using inercya.EntityLite.Extensions;
using inercya.EntityLite.Collections;
using Microsoft.Extensions.Logging;

namespace inercya.EntityLite.Builders
{

	public class CommandBuilder : IDisposable
	{
        private static ILogger logger;

		private static ILogger Log
        {
            get
            {
                if (logger == null)
                {
                    logger = ConfigurationLite.LoggerFactory.CreateLogger<CommandBuilder>();
                }
                return logger;
            }
        } 

		public readonly DataService DataService;

		public CommandBuilder(DataService dataService)
		{
			this.DataService = dataService;
		}

        private Dictionary<Type, string, DbCommand> updateCommandCache = new Dictionary<Type, string, DbCommand>();

        private Dictionary<Type, DbCommand> insertCommandCache = new Dictionary<Type, DbCommand>();

        private Dictionary<Type, DbCommand> deleteCommandCache = new Dictionary<Type, DbCommand>();


        public DbCommand GetUpdateCommand(object entity, IEnumerable<string> fieldsToUpdate, EntityMetadata entityMetadata)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();
            string fieldsKey = fieldsToUpdate.Join();
            DbCommand command;
            if (updateCommandCache.TryGetValue(entityType, fieldsKey, out command))
            {
                SetParameterValuesFromObject(command, entity, entityMetadata);
            }
            else
            {
                command = GenerateUpdateCommand(entity, fieldsToUpdate);
                updateCommandCache.Add(entityType, fieldsKey, command);
            }
            return command;
        }

		public  DbCommand GetInsertCommand(object entity, EntityMetadata entityMetadata)
		{
			Type entityType = entityMetadata.EntityType;
			DbCommand command;
			if (insertCommandCache.TryGetValue(entityType, out command))
			{
				SetParameterValuesFromObject(command, entity, entityMetadata);
			}
			else
            {
				command = GenerateInsertCommand(entity);
                insertCommandCache.Add(entityType, command);
			}
            if (entityMetadata.IsPrimaryKeyGuid)
            {
                Guid primaryKeyValue = (Guid)entity.GetPropertyValue(entityMetadata.PrimaryKeyPropertyName);
                if (primaryKeyValue == Guid.Empty)
                {
                    command.Parameters[this.DataService.EntityLiteProvider.ParameterPrefix + entityMetadata.PrimaryKeyPropertyName].Value = this.DataService.NewGuid();
                }
            }
			return command;
		}

        public DbCommand GetDeleteCommand(object entity, EntityMetadata entityMetadata)
        {
            Type entityType = entity.GetType();
            DbCommand command;
            if (deleteCommandCache.TryGetValue(entityType, out command))
            {
                SetParameterValuesFromObject(command, entity, entityMetadata);
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

            string fullTableName = entityMetadata.GetFullTableName(this.DataService.EntityLiteProvider.DefaultSchema, this.DataService.EntityLiteProvider.StartQuote, this.DataService.EntityLiteProvider.EndQuote);

            IPropertyGetterDictionary getters = PropertyHelper.GetPropertyGetters(entityType);
            DbCommand cmd = DataService.EntityLiteProvider.CreateCommand();
            StringBuilder commandText = new StringBuilder();
            commandText.Append("\nUPDATE ").Append(fullTableName);
            
            bool hasEntityRowVersionField = false;
            GenerateUpdateSetClause(entity, fieldsToUpdate, entityMetadata, getters, cmd, commandText, ref hasEntityRowVersionField);
            GenerateUpdateWhereClauseWithPrimaryKeyAndEntityRowVersion(entity, entityMetadata, getters, cmd, commandText, hasEntityRowVersionField);
            AddModifiedFieldsPredicateToToUpdateWhereClause(fieldsToUpdate, entityMetadata, commandText);
            cmd.CommandText = commandText.ToString();
            return cmd;
        }

        private void AddModifiedFieldsPredicateToToUpdateWhereClause(IEnumerable<string> fieldsToUpdate, EntityMetadata entityMetadata, StringBuilder commandText)
        {
            var propertiesMetadata = new List<PropertyMetadata>();
            foreach (var propertyName in fieldsToUpdate)
            {
                PropertyMetadata propMetadata = null;
                if (!entityMetadata.UpdatableProperties.TryGetValue(propertyName, out propMetadata)) continue;
                SqlFieldAttribute field = propMetadata.SqlField;
                if ((field.Size == Int32.MaxValue) && (DataService.EntityLiteProvider is inercya.EntityLite.Providers.OracleEntityLiteProvider))
                {
                    return;
                }
                if (!field.IsKey
                    && !string.Equals(propertyName, DataService.SpecialFieldNames.CreatedByFieldName, StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(propertyName, DataService.SpecialFieldNames.CreatedDateFieldName, StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(propertyName, DataService.SpecialFieldNames.EntityRowVersionFieldName, StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(propertyName, DataService.SpecialFieldNames.ModifiedByFieldName, StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(propertyName, DataService.SpecialFieldNames.ModifiedDateFieldName, StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(propertyName, DataService.SpecialFieldNames.DbChangeNumberFieldName, StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    propertiesMetadata.Add(propMetadata);
                }
            }

            if (propertiesMetadata.Count == 0) return;

            bool firstTime = true;
            commandText.Append("\n    AND (");

            foreach (var propMetadata in propertiesMetadata)
            {
                SqlFieldAttribute field = propMetadata.SqlField;
                string propertyName = propMetadata.PropertyInfo.Name;
                string parameterName = DataService.EntityLiteProvider.ParameterPrefix + propertyName;
                string fieldName = this.DataService.EntityLiteProvider.StartQuote + field.BaseColumnName + this.DataService.EntityLiteProvider.EndQuote;
                commandText.Append("\n        ");
                if (firstTime) firstTime = false;
                else commandText.Append("OR ");
                if (propMetadata.SqlField.DbType == DbType.Xml)
                {
                    commandText.Append("CAST (").Append(fieldName).Append(" AS nvarchar(max))");
                    commandText.Append(" <> ");
                    commandText.Append("CAST (").Append(parameterName).Append(" AS nvarchar(max))");
                }
                else
                {
                    commandText.Append(fieldName).Append(" <> ").Append(parameterName);
                }
                commandText.Append(" OR ").Append(fieldName).Append(" IS NULL AND ").Append(parameterName).Append(" IS NOT NULL")
                    .Append(" OR ").Append(fieldName).Append(" IS NOT NULL AND ").Append(parameterName).Append(" IS NULL");
            }
            commandText.Append("\n    )");

        }

        private void GenerateUpdateWhereClauseWithPrimaryKeyAndEntityRowVersion(object entity, EntityMetadata entityMetadata, IPropertyGetterDictionary getters, DbCommand cmd, StringBuilder commandText, bool hasEntityRowVersionField)
        {
            bool firstTime = true;

            IEnumerable<string> whereFields = entityMetadata.PrimaryKeyPropertyNames;
            if (hasEntityRowVersionField) whereFields = whereFields.Concat(new string[] { DataService.SpecialFieldNames.EntityRowVersionFieldName });
            foreach (string whereField in whereFields)
            {
                var property = entityMetadata.Properties[whereField];
                SqlFieldAttribute field = property.SqlField;
                if (string.Equals(field.ColumnName, field.BaseColumnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    string parameterName;
                    IDbDataParameter parameter = CreateParameter(property, whereField, out parameterName);
                    if (firstTime)
                    {
                        commandText.Append("\nWHERE\n    ");
                        firstTime = false;
                    }
                    else
                    {
                        commandText.Append("\n    AND ");
                    }
                    commandText.Append(this.DataService.EntityLiteProvider.StartQuote + field.BaseColumnName + this.DataService.EntityLiteProvider.EndQuote)
                        .Append(" = ").Append(parameterName);

                    
                    PropertyGetter getter;
                    if (getters.TryGetValue(whereField, out getter))
                    {
                        object parameterValue = getter(entity);
                        parameter.Value = parameterValue == null ? DBNull.Value : parameterValue;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        private void GenerateUpdateSetClause(object entity, IEnumerable<string> fieldsToUpdate, EntityMetadata entityMetadata, IPropertyGetterDictionary getters, DbCommand cmd, StringBuilder commandText, ref bool hasEntityRowVersionField)
        {
            bool firstTime = true;
            foreach (var propertyName in fieldsToUpdate)
            {
                PropertyMetadata propMetadata = null;
                if (!entityMetadata.UpdatableProperties.TryGetValue(propertyName, out propMetadata)) continue;
                SqlFieldAttribute field = propMetadata.SqlField;
                if (!field.IsKey
                    && !string.Equals(propertyName, DataService.SpecialFieldNames.CreatedByFieldName, StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(propertyName, DataService.SpecialFieldNames.CreatedDateFieldName, StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    if (firstTime)
                    {
                        commandText.Append(" SET\n    ");
                        firstTime = false;
                    }
                    else
                    {
                        commandText.Append(",\n    ");
                    }
                    if (string.Equals(propertyName, DataService.SpecialFieldNames.EntityRowVersionFieldName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        hasEntityRowVersionField = true;
                        commandText.Append(this.DataService.EntityLiteProvider.StartQuote + propMetadata.SqlField.BaseColumnName + this.DataService.EntityLiteProvider.EndQuote).Append(" = ")
                            .Append(this.DataService.EntityLiteProvider.StartQuote + propMetadata.SqlField.BaseColumnName + this.DataService.EntityLiteProvider.EndQuote).Append(" + 1");
                    }
                    else if (propertyName == DataService.SpecialFieldNames.DbChangeNumberFieldName)
                    {
                        commandText.Append(this.DataService.EntityLiteProvider.StartQuote + propMetadata.SqlField.BaseColumnName + this.DataService.EntityLiteProvider.EndQuote).Append(" = ")
                            .Append(DataService.EntityLiteProvider.GetNextValExpression(GetDbChangeNumberFullSequenceName()));
                    }
                    else
                    {
                        string parameterName;
                        IDbDataParameter param = CreateParameter(propMetadata, propertyName, out parameterName);
                        commandText.Append(this.DataService.EntityLiteProvider.StartQuote + field.BaseColumnName + this.DataService.EntityLiteProvider.EndQuote)
                            .Append(" = ").Append(parameterName);
                        
                        object fieldValue = getters[propertyName](entity);
                        SetValueToCommandParameter(entity, getters, propertyName, param);
                        cmd.Parameters.Add(param);
                    }
                }
            }
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

            return DataService.EntityLiteProvider.GenerateInsertCommand(this, entity);
		}


        public void AppendInsertStatement(object entity, DbCommand cmd, StringBuilder commandText)
        {
            var entityType = entity.GetType();
            var entityMetadata = entityType.GetEntityMetadata();
            string fullTableName = entityMetadata.GetFullTableName(this.DataService.EntityLiteProvider.DefaultSchema, this.DataService.EntityLiteProvider.StartQuote, this.DataService.EntityLiteProvider.EndQuote);
            IPropertyGetterDictionary getters = entityType.GetPropertyGetters();
            var entityRowVersionFieldName = DataService.SpecialFieldNames.EntityRowVersionFieldName;

            StringBuilder valuesText = new StringBuilder();
            commandText.Append("\nINSERT INTO  ").Append(fullTableName);
            bool firstTime = true;
            var sequenceVariable = DataService.EntityLiteProvider.SequenceVariable;
            var startQuote = this.DataService.EntityLiteProvider.StartQuote;
            var parameters = cmd.Parameters;
            var endQuote = this.DataService.EntityLiteProvider.EndQuote;
            var parameterPrefix = this.DataService.EntityLiteProvider.ParameterPrefix;
            foreach (var kv in entityMetadata.UpdatableProperties)
            {
                SqlFieldAttribute field = kv.Value.SqlField;
                //en PostgeSQL se manejan las secuencias de otra manera, como un autonumérico.
                //
                //if (field.SequenceName != null && sequenceVariable == null) continue; 
                string propertyName = kv.Key;
                if (firstTime)
                {
                    commandText.Append("(");
                    valuesText.Append("\nVALUES (");
                    firstTime = false;
                }
                else
                {
                    commandText.Append(", ");
                    valuesText.Append(", ");
                }
                commandText.Append(startQuote).Append(field.BaseColumnName).Append(endQuote);
                
                if (propertyName == entityRowVersionFieldName)
                {
                    valuesText.Append("1");
                }
                else if (propertyName == DataService.SpecialFieldNames.DbChangeNumberFieldName)
                {
                    valuesText.Append(DataService.EntityLiteProvider.GetNextValExpression(GetDbChangeNumberFullSequenceName()));
                }
                else if (field.SequenceName != null)
                {
                    if (DataService.EntityLiteProvider.SequenceVariable != null)
                    {
                        valuesText.Append(DataService.EntityLiteProvider.SequenceVariable);
                    }
                    else
                    {
                        valuesText.Append(DataService.EntityLiteProvider.GetNextValExpression(field.SequenceName));
                    }
                }
                else
                {
                    string parameterName;
                    IDbDataParameter param = CreateParameter(kv.Value, propertyName, out parameterName);
                    valuesText.Append(parameterName);
                    SetValueToCommandParameter(entity, getters, propertyName, param);
                    parameters.Add(param);
                }
            }
            commandText.Append(")");
            valuesText.Append(")");
            commandText.Append(valuesText.ToString());
        }

        private string GetDbChangeNumberFullSequenceName()
        {
            var sequenceName = DataService.SpecialFieldNames.DbChangeNumberFieldName;
            if (!string.IsNullOrEmpty(DataService.SequencePrefix))
            {
                sequenceName = DataService.SequencePrefix + sequenceName;
            }
            else if (!string.IsNullOrEmpty(DataService.SequenceSuffix))
            {
                sequenceName = sequenceName + DataService.SequenceSuffix;
            }
            sequenceName = sequenceName.Transform(DataService.EntityNameToEntityViewTransform);
            if (!string.IsNullOrEmpty(DataService.EntityLiteProvider.DefaultSchema))
            {
                sequenceName = DataService.EntityLiteProvider.DefaultSchema + "." + sequenceName;
            }
            return sequenceName;
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



		private void SetParameterValuesFromObject(DbCommand command, object obj, EntityMetadata entityMetadata)
		{
			IPropertyGetterDictionary getters = entityMetadata.Getters;
            var parameters = command.Parameters;
            var count = parameters.Count;
			for (int i = 0; i < count; i++)
			{
                var param = parameters[i];
				if ((param.Direction & ParameterDirection.Input) == ParameterDirection.Input)
				{
					SetValueToCommandParameter(obj, getters, param.SourceColumn, param);
				}
			}
		}

		protected IDbDataParameter CreateParameter(PropertyMetadata property, string fieldName, out string parameterName)
		{
			var field = property.SqlField;
			parameterName = DataService.EntityLiteProvider.ParameterPrefix + fieldName;
			IDbDataParameter parameter = DataService.DbProviderFactory.CreateParameter();
			parameter.ParameterName = parameterName;
            parameter.SourceColumn = fieldName;
            Type propertyType = property.PropertyInfo.PropertyType.UndelyingType();
            
			if (typeof(System.Data.SqlTypes.INullable).IsAssignableFrom(propertyType))
			{
				parameter.DbType = DbType.String;
                if (propertyType.FullName == "Microsoft.SqlServer.Types.SqlHierarchyId")
                {
                    parameter.Size = 4000;
                }
                else
                {
                    parameter.Size = 1073741823;
                }
                return parameter;
			}
            if (field.DbType == DbType.Time && parameter is System.Data.SqlClient.SqlParameter)
            {
                ((System.Data.SqlClient.SqlParameter)parameter).SqlDbType = SqlDbType.Time;
            }
            else if (field.ProviderType != int.MaxValue)
            {
                DataService.EntityLiteProvider.SetProviderTypeToParameter(parameter, field.ProviderType);
            }
            else
            {
                parameter.DbType = field.DbType;
            }
			parameter.Size = field.Size;
			
			if (field.Precision != 255 && field.Precision != 0)
			{
				parameter.Precision = field.Precision;
			}
			if (field.Scale != 255 && field.Scale != 0)
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
				DbCommand cmd = DataService.EntityLiteProvider.CreateCommand();
				StringBuilder commandText = new StringBuilder();
                string fullTableName = entityMetadata.GetFullTableName(this.DataService.EntityLiteProvider.DefaultSchema, this.DataService.EntityLiteProvider.StartQuote, this.DataService.EntityLiteProvider.EndQuote);
				commandText.Append("\nDELETE FROM ").Append(fullTableName);
				bool firstTime = true;
				foreach (string fieldName in entityMetadata.PrimaryKeyPropertyNames)
				{
					var property = entityMetadata.Properties[fieldName];
					SqlFieldAttribute field = property.SqlField;
                    string parameterName;
                    IDbDataParameter param = CreateParameter(property, fieldName, out parameterName);
                    SetValueToCommandParameter(entity, getters, fieldName, param);
                    cmd.Parameters.Add(param);
                    if (firstTime)
					{
						commandText.Append("\nWHERE\n    ");
						firstTime = false;
					}
					else
					{
						commandText.Append("\n    AND ");
					}
					commandText.Append(this.DataService.EntityLiteProvider.StartQuote + field.BaseColumnName + this.DataService.EntityLiteProvider.EndQuote).Append(" = ").Append(parameterName);
				}
				cmd.CommandText = commandText.ToString();
				return cmd;
			}
			catch (Exception ex)
			{
				if (entity == null)
				{
					Log.LogError(ex, "Error generating delete command");
				}
				else
				{
					Log.LogError(ex, string.Format("Error generating delete command for entity of type {0} with primary key: {1}", entity.GetType().Name, entity.GetPrimaryKey().ToListString() ?? "{no id}"));
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
