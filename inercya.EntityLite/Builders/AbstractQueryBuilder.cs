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

using inercya.EntityLite.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;


namespace inercya.EntityLite.Builders
{
    public abstract class AbstractQueryBuilder : IQueryBuilder 
    {

        protected AbstractQueryBuilder(IQueryLite queryLite)
        {
            this.QueryLite = queryLite;
        }

        public IQueryLite QueryLite { get; private set; }

		public void SetOptions(StringBuilder commandText)
		{
            if (commandText == null) throw new ArgumentNullException(nameof(commandText));
			if (QueryLite.Options != null && QueryLite.Options.Count > 0)
			{
				commandText.Append("\nOPTION (");
				bool firstTime = true;
				foreach (var option in QueryLite.Options)
				{
					if (firstTime) firstTime = false;
					else commandText.Append(", ");
					commandText.Append(option);
				}
				commandText.Append(')');
			}
		}

        public virtual string GetColumnList()
        {
            return string.Join(", ", this.GetColumns(this.QueryLite.EntityType, QueryLite.FieldList));
        }

        public virtual string[] GetColumns(Type entityType, IList<string> propertyNames)
        {
            if (propertyNames == null || propertyNames.Count == 0 || propertyNames[0] == "*")
            {
                return new string[] { "*" };
            }
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            var properties = entityType.GetEntityMetadata().Properties;
            string startQuote = QueryLite.DataService.EntityLiteProvider.StartQuote;
            string endQuote = QueryLite.DataService.EntityLiteProvider.EndQuote;
            var columns = new string[propertyNames.Count];
            for (int i = 0; i < columns.Length; i++)
            {
                var fieldName = propertyNames[i];
                if (!properties.TryGetValue(fieldName, out var propertyMetadata))
                {
                    var message = string.Format(CultureInfo.InvariantCulture, "property '{0}' not found in type '{1}'", fieldName, entityType.Name);
                    throw new KeyNotFoundException(message);
                }
                if (propertyMetadata.IsLocalizedFiled)
                {
                    propertyMetadata = properties[CurrentLanguageService.GetSufixedLocalizedFieldName(fieldName)];
                }
                columns[i] = startQuote + propertyMetadata.SqlField.ColumnName + endQuote;
            }
            return columns;
        }



        public void GetSelectQuery(DbCommand selectCommand, ref int paramIndex, StringBuilder commandText, int indentation, bool addSelectKeyword = true)
        {
            if (selectCommand == null) throw new ArgumentNullException(nameof(selectCommand));
            if (commandText == null) throw new ArgumentNullException(nameof(commandText));
            commandText.Indent(indentation);
            var columnList = GetColumnList();
            bool isStar = columnList == "*";
            if (addSelectKeyword) commandText.Append("SELECT ");
            if (isStar) commandText.Append('*');
            else commandText.NewIndentedLine(++indentation).Append(columnList);
            if (!isStar) indentation--;    
            commandText.NewIndentedLine(indentation).Append("FROM ")
                .NewIndentedLine(++indentation).Append(GetFromClauseContent(selectCommand, ref paramIndex, indentation));
            indentation--;
            bool hasWhereClause = QueryLite.Filter != null && !QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.NewIndentedLine(indentation).Append("WHERE");
                commandText.NewIndentedLine(++indentation).Append(GetFilter(selectCommand, ref paramIndex, QueryLite.Filter, indentation, false));
                indentation--;
            }
            bool hasOrderbyClause = QueryLite.Sort != null && QueryLite.Sort.Count > 0;
            if (hasOrderbyClause)
            {
                commandText.NewIndentedLine(indentation).Append("ORDER BY");
                commandText.NewIndentedLine(++indentation).Append(GetSort());
                indentation--;
            }
        }

        public void GetDeleteQuery(DbCommand cmd, ref int paramIndex, StringBuilder commandText, int indentation)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));
            if (commandText == null) throw new ArgumentNullException(nameof(commandText));

            commandText.Indent(indentation);
            commandText.Append("DELETE FROM ").Append(GetFromClauseContent(cmd, ref paramIndex, indentation));
            bool hasWhereClause = QueryLite.Filter != null && !QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.NewIndentedLine(indentation).Append("WHERE");
                commandText.NewIndentedLine(++indentation)
                    .Append(GetFilter(cmd, ref paramIndex, QueryLite.Filter, indentation, false));
            }
        }

        public string GetDeleteQuery(DbCommand cmd, ref int paramIndex, int indentation)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));
            StringBuilder commandText = new StringBuilder();
            GetDeleteQuery(cmd, ref paramIndex, commandText, indentation);
            var query = commandText.ToString();
            return query;
        }

        public void GetSelectIntoQuery(DbCommand selectCommand, ref int paramIndex, StringBuilder commandText, int indentation, string destinationTableName)
        {
            if (selectCommand == null) throw new ArgumentNullException(nameof(selectCommand));
            if (commandText == null) throw new ArgumentNullException(nameof(commandText));
            if (destinationTableName == null) throw new ArgumentNullException(nameof(destinationTableName));
            commandText.Indent(indentation);
            var columnList = GetColumnList();
            bool isStar = columnList == "*";
            commandText.Append("SELECT ");
            if (isStar) commandText.Append('*');
            else commandText.NewIndentedLine(++indentation).Append(columnList);
            if (!isStar) indentation--;

           
            string startQuote = this.QueryLite.DataService.EntityLiteProvider.StartQuote;
            string endQuote = this.QueryLite.DataService.EntityLiteProvider.EndQuote;
            string quotedObjectName = null;
            if (destinationTableName.StartsWith(startQuote, StringComparison.Ordinal) && destinationTableName.EndsWith(endQuote, StringComparison.Ordinal))
            {
                quotedObjectName = destinationTableName;
            }
            else
            {
                var tokens = destinationTableName.Split('.');
                quotedObjectName = string.Join(".", tokens.Select(x => startQuote + x + endQuote).ToArray());
            }
            commandText.NewIndentedLine(indentation).Append("INTO ").Append(quotedObjectName);
            commandText.NewIndentedLine(indentation).Append("FROM ")
                .NewIndentedLine(++indentation).Append(GetFromClauseContent(selectCommand, ref paramIndex, indentation));
            indentation--;
            bool hasWhereClause = QueryLite.Filter != null && !QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.NewIndentedLine(indentation).Append("WHERE");
                commandText.NewIndentedLine(++indentation).Append(GetFilter(selectCommand, ref paramIndex, QueryLite.Filter, indentation, false));
                indentation--;
            }
            bool hasOrderbyClause = QueryLite.Sort != null && QueryLite.Sort.Count > 0;
            if (hasOrderbyClause)
            {
                commandText.NewIndentedLine(indentation).Append("ORDER BY");
                commandText.NewIndentedLine(++indentation).Append(GetSort());
                indentation--;
            }
        }

        public string GetSelectIntoQuery(DbCommand cmd, ref int paramIndex, int indentation, string destinationTableName)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));
            if (destinationTableName == null) throw new ArgumentNullException(nameof(destinationTableName));
            StringBuilder commandText = new StringBuilder();
            GetSelectIntoQuery(cmd, ref paramIndex, commandText, indentation, destinationTableName);
            SetOptions(commandText);
            var query = commandText.ToString();
            return query;
        }

        public void GetInsertIntoQuery(DbCommand selectCommand, ref int paramIndex, StringBuilder commandText, int indentation, string destinationTableName, string[] columnNames)
        {
            if (selectCommand == null) throw new ArgumentNullException(nameof(selectCommand));
            if (commandText == null) throw new ArgumentNullException(nameof(commandText));
            if (destinationTableName == null) throw new ArgumentNullException(nameof(destinationTableName));
            string startQuote = this.QueryLite.DataService.EntityLiteProvider.StartQuote;
            string endQuote = this.QueryLite.DataService.EntityLiteProvider.EndQuote;
            string quotedObjectName = null;
            if (destinationTableName.StartsWith(startQuote, StringComparison.Ordinal) && destinationTableName.EndsWith(endQuote, StringComparison.Ordinal))
            {
                quotedObjectName = destinationTableName;
            }
            else
            {
                var tokens = destinationTableName.Split('.');
                quotedObjectName = string.Join(".", tokens.Select(x => startQuote + x + endQuote).ToArray());
            }

            commandText.Indent(indentation);
            commandText.Append("INSERT INTO ").Append(quotedObjectName);
            if (columnNames != null && columnNames.Length > 0)
            {
                commandText.Append('(');
                var firstTime = true;
                foreach (var column in columnNames)
                {
                    if (firstTime) firstTime = false;
                    else commandText.Append(", ");
                    if (column.StartsWith(startQuote, StringComparison.Ordinal) && column.EndsWith(endQuote, StringComparison.Ordinal))
                    {
                        commandText.Append(column);
                    }
                    else
                    {
                        commandText.Append(startQuote).Append(column).Append(endQuote);
                    }
                }
                commandText.Append(')');
            }

            commandText.NewIndentedLine(indentation);
            this.GetSelectQuery(selectCommand, ref paramIndex, commandText, indentation);
        }

        public string GetInsertIntoQuery(DbCommand cmd, ref int paramIndex, int indentation, string destinationTableName, string[] columnNames)
        {
            StringBuilder commandText = new StringBuilder();
            GetInsertIntoQuery(cmd, ref paramIndex, commandText, indentation, destinationTableName, columnNames);
            SetOptions(commandText);
            var query = commandText.ToString();
            return query;
        }


        public string GetSelectQuery(DbCommand cmd, ref int paramIndex, int indentation)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));
            StringBuilder commandText = new StringBuilder();
            GetSelectQuery(cmd, ref paramIndex, commandText, indentation);
            SetOptions(commandText);
            var query = commandText.ToString();
            return query;
        }


        string IQueryBuilder.GetSelectQuery(DbCommand selectCommand, ref int paramIndex, int fromRowIndex, int toRowIndex, int indentation)
        {
            return this.QueryLite.DataService.EntityLiteProvider.GetPagedQuery(this, selectCommand, ref paramIndex, fromRowIndex, toRowIndex);
        }

        string IQueryBuilder.GetCountQuery(DbCommand selectCommand, ref int paramIndex)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT COUNT(*) \nFROM \n   ").Append(GetFromClauseContent(selectCommand, ref paramIndex, 1));
            bool hasWhereClause = QueryLite.Filter != null && !QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.Append("\nWHERE\n   ").Append(GetFilter(selectCommand, ref paramIndex, QueryLite.Filter, 1, false));
            }
            SetOptions(commandText);
            return commandText.ToString();
        }

        string IQueryBuilder.GetAnyQuery(DbCommand selectCommand, ref int paramIndex)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("\nSELECT CASE WHEN EXISTS (SELECT * FROM ").Append(GetFromClauseContent(selectCommand, ref paramIndex, 1));
            bool hasWhereClause = QueryLite.Filter != null && !QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.Append("\nWHERE\n   ").Append(GetFilter(selectCommand, ref paramIndex, QueryLite.Filter, 1, false));
            }
            commandText.Append(") THEN 1 ELSE 0 END AS HasAnyRow");
            var dualTable = this.QueryLite.DataService.EntityLiteProvider.DualTable;
            if (!string.IsNullOrEmpty(dualTable))
            {
                commandText.Append("\nFROM ").Append(dualTable);
            }
            SetOptions(commandText);
            return commandText.ToString();
        }

        private static readonly PropertySetter udtTypeNameSetter = PropertyHelper.GetPropertySetter(typeof(SqlParameter), "UdtTypeName");

        protected IDbDataParameter CreateParameter(PropertyMetadata propertyMetadata, object paramValue, ref int paramIndex, out string parameterName)
        {
            if (propertyMetadata == null) throw new ArgumentNullException(nameof(propertyMetadata));
            parameterName = QueryLite.DataService.EntityLiteProvider.ParameterPrefix + "P" + paramIndex.ToString(CultureInfo.InvariantCulture);
            IDbDataParameter parameter = QueryLite.DataService.DbProviderFactory.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Size = propertyMetadata.SqlField.Size;

            Type propertyType = propertyMetadata.PropertyInfo.PropertyType.UndelyingType();
            var sqlParam = parameter as SqlParameter;
            if (propertyType.FullName.StartsWith("Microsoft.SqlServer.Types.Sql", StringComparison.Ordinal))
            {
                if (sqlParam != null)
                {
                    sqlParam.SqlDbType = SqlDbType.Udt;
                    //TODO: En .NET Core no va a funcionar porque no existe la propiedad "UdtTypeName" y dará un error en tiempo de ejecución.
#pragma warning disable CA1308 // Normalize strings to uppercase
                    udtTypeNameSetter?.Invoke(sqlParam, propertyType.Name.ToLowerInvariant());
#pragma warning restore CA1308 // Normalize strings to uppercase
                }
                else
                {
                    parameter.DbType = DbType.String;
                    if (propertyType.Name.StartsWith("SqlHierarchyId", StringComparison.Ordinal))
                    {
                        parameter.Size = 4000;
                    }
                    else
                    {
                        parameter.Size = 1073741823;
                    }
                }
            }
            else
            {

                if (propertyMetadata.SqlField.ProviderType != int.MaxValue)
                {
                    this.QueryLite.DataService.EntityLiteProvider.SetProviderTypeToParameter(parameter, propertyMetadata.SqlField.ProviderType);
                }
                else if (propertyMetadata.SqlField.DbType == DbType.AnsiStringFixedLength)
                {
                    parameter.DbType = DbType.AnsiString;
                }
                else if (propertyMetadata.SqlField.DbType == DbType.StringFixedLength)
                {
                    parameter.DbType = DbType.String;
                }
                else if (propertyMetadata.SqlField.DbType == DbType.Time && sqlParam != null)
                {
                    sqlParam.SqlDbType = SqlDbType.Time;
                }
                else
                {
                    parameter.DbType = propertyMetadata.SqlField.DbType;
                }
                parameter.SourceColumn = propertyMetadata.PropertyInfo.Name;
                if (propertyMetadata.SqlField.Precision != 255 && propertyMetadata.SqlField.Precision != 0)
                {
                    parameter.Precision = propertyMetadata.SqlField.Precision;
                }
                if (propertyMetadata.SqlField.Scale != 255)
                {
                    parameter.Scale = propertyMetadata.SqlField.Scale;
                }
            }

            if (paramValue != null && (parameter.DbType == DbType.String || parameter.DbType == DbType.AnsiString) && !(paramValue is string))
            {
				var jsonNode = paramValue as JsonNode;
                if (jsonNode != null)
                {
                    paramValue = jsonNode.ToJsonString(DefaultJsonSerializerOptions.Instance);
                }
                else
                {
                    var convertible = paramValue as IConvertible;
                    if (convertible != null)
                    {
                        paramValue = convertible.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        paramValue = paramValue.ToString();
                    }
                }
            }
            INullable sqlNullable = paramValue as INullable;
            parameter.Value = (paramValue == null || (sqlNullable != null && sqlNullable.IsNull)) ? DBNull.Value : paramValue;
            paramIndex++;
            return parameter;
        }

        public IDbDataParameter CreateIn32Parameter(int paramValue, ref int paramIndex, out string parameterName)
        {
            parameterName = QueryLite.DataService.EntityLiteProvider.ParameterPrefix + "P" + paramIndex.ToString(CultureInfo.InvariantCulture);
            IDbDataParameter parameter = QueryLite.DataService.DbProviderFactory.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = DbType.Int32;
            parameter.Value = paramValue;
            paramIndex++;
            return parameter;
        }

        private static PropertyMetadata GetPropertyMetadata(Type entityType, string fieldName)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));
            PropertyMetadata propertyMetadata;
            EntityMetadata entityMetadata = entityType.GetEntityMetadata();
            if (entityMetadata == null) throw new InvalidOperationException("Entity " + entityType.Name + " has no metadata");
            if (!entityMetadata.Properties.TryGetValue(fieldName, out propertyMetadata))
            {
                throw new ArgumentException("Field " + fieldName + " cannot be used in a filter because it is not a property of " + entityType.Name);
            }
            if (propertyMetadata.IsLocalizedFiled)
            {
                fieldName = CurrentLanguageService.GetSufixedLocalizedFieldName(fieldName);

                if (string.IsNullOrEmpty(fieldName))
                {
                    throw new InvalidOperationException("Cannot sort by localized property " + fieldName + " because no field name has been found for the current language: " + CurrentLanguageService.CurrentLanguageCode);
                }
                if (!entityMetadata.Properties.TryGetValue(fieldName, out propertyMetadata))
                {
                    throw new ArgumentException("Field " + fieldName + " cannot be used in a filter because it is not a property of " + entityType.Name);
                }
            }
            if (propertyMetadata.SqlField == null)
            {
                throw new ArgumentException("Field " + fieldName + " cannot be used in a filter because it has no metadata");
            }
            return propertyMetadata;
        }


        protected virtual void GenerateFilterForCondition(DbCommand cmd, ConditionLite condition, StringBuilder sb, ref int paramIndex, ref bool firstCondition, int indentation)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));
            if (sb == null) throw new ArgumentNullException(nameof(sb));
            string parameterName = null;
            if (condition.Filter != null && condition.Filter.IsEmpty()) return;
            if (firstCondition) { ;}
            else if (condition.LogicalOperator == LogicalOperatorLite.And) sb.NewIndentedLine(indentation).Append("AND ");
            else if (condition.LogicalOperator == LogicalOperatorLite.Or)  sb.NewIndentedLine(indentation).Append("OR ");
            else throw new NotImplementedException("Logical operator " + condition.LogicalOperator.ToString() + " not implemented");
            firstCondition = false;

            if (condition.Filter != null)
            {
                sb.Append(GetFilter(cmd, ref paramIndex, condition.Filter, indentation, true));
                return;
            }

            IEnumerable values = condition.FieldValue as IEnumerable;
            IQueryBuilder queryBuilder = condition.SubQuery == null ? null : condition.SubQuery.QueryBuilder;
            if (condition.Operator == OperatorLite.In || condition.Operator == OperatorLite.NotIn)
            {
                if (values == null && queryBuilder == null) throw new ArgumentException("The value for In and NotIn operators must be enumerable or a subquery", nameof(condition));
                if (values != null)
                {
                    bool hasAnyValue = values.AreThereMoreThan(0);
                    if (!hasAnyValue)
                    {
                        if (condition.Operator == OperatorLite.In)
                        {
                            sb.Append(" 1=0");
                        }
                        else
                        {
                            sb.Append(" 1=1");
                        }
                        return;
                    }
                }
            }
            PropertyMetadata propertyMetadata = null;
			string fieldName = condition.FieldName;
            string quotedColumnName = null;

            if (fieldName == null && condition.Operator != OperatorLite.Exists && condition.Operator != OperatorLite.NotExists)
            {
                throw new InvalidOperationException("Field Name must be not null for condition");
            }
            if (fieldName != null)
            {
                propertyMetadata = GetPropertyMetadata(this.QueryLite.EntityType, fieldName);
                quotedColumnName = this.QueryLite.DataService.EntityLiteProvider.StartQuote + propertyMetadata.SqlField.ColumnName + this.QueryLite.DataService.EntityLiteProvider.EndQuote;
                if (condition.Operator != OperatorLite.In && condition.Operator != OperatorLite.NotIn) sb.Append(quotedColumnName);
            }

            if (condition.Operator == OperatorLite.IsNull)
            {
                sb.Append(" IS NULL");
                return;
            }
            if (condition.Operator == OperatorLite.IsNotNull)
            {
                sb.Append(" IS NOT NULL");
                return;
            }

            if (condition.Operator == OperatorLite.In || condition.Operator == OperatorLite.NotIn)
            {
                AddListCondition(cmd, condition, sb, ref paramIndex, ref indentation,  values, queryBuilder, propertyMetadata, quotedColumnName);
                return;
            }
            else if (condition.Operator == OperatorLite.Exists || condition.Operator == OperatorLite.NotExists)
            {
                if (condition.Operator == OperatorLite.Exists) sb.Append("EXISTS (\n");
                else sb.Append("NOT EXISTS (\n");
                sb.Append(queryBuilder.GetSelectQuery(cmd, ref paramIndex, ++indentation));
                sb.NewIndentedLine(--indentation).Append(')');
                return;
            }

            var fieldReference = condition.FieldValue as FieldReference;

            if (fieldReference == null)
            {
                var parameter = CreateParameter(propertyMetadata, condition.FieldValue, ref paramIndex, out parameterName);
                cmd.Parameters.Add(parameter);
            }
            else
            {
                var fieldReferenceEntityType = fieldReference.Alias?.EntityType ?? this.QueryLite.EntityType;
                string fieldReferenceName = fieldReference.FieldName;
                PropertyMetadata fieldReferencePropertyMetadata = GetPropertyMetadata(fieldReferenceEntityType, fieldReferenceName);
                var aliasName = fieldReference.Alias?.Name ?? this.QueryLite.Alias?.Name;
                if (!string.IsNullOrEmpty(aliasName)) parameterName = aliasName + ".";
                else parameterName = string.Empty;
                parameterName += this.QueryLite.DataService.EntityLiteProvider.StartQuote + fieldReferencePropertyMetadata.SqlField.ColumnName + this.QueryLite.DataService.EntityLiteProvider.EndQuote;
            }


            switch (condition.Operator)
            {
                case OperatorLite.Contains:
                    sb.Append(" LIKE " + QueryLite.DataService.EntityLiteProvider.Concat("'%'", parameterName, "'%'"));
                    break;
                case OperatorLite.FbContaining:
                    sb.Append(" CONTAINING ").Append(parameterName);
                    break;
                case OperatorLite.FbNotContaining:
                    sb.Append(" NOT CONTAINING ").Append(parameterName);
                    break;
                case OperatorLite.Equals:
                    sb.Append(" = ").Append(parameterName);
                    break;
                case OperatorLite.Greater:
                    sb.Append(" > ").Append(parameterName);
                    break;
                case OperatorLite.GreaterOrEquals:
                    sb.Append(" >= ").Append(parameterName);
                    break;
                case OperatorLite.Less:
                    sb.Append(" < ").Append(parameterName);
                    break;
                case OperatorLite.LessOrEquals:
                    sb.Append(" <= ").Append(parameterName);
                    break;
                case OperatorLite.NotContains:
                    sb.Append(" NOT LIKE " + QueryLite.DataService.EntityLiteProvider.Concat("'%'", parameterName, "'%'"));
                    break;
                case OperatorLite.NotEquals:
                    sb.Append(" <> ").Append(parameterName);
                    break;
                case OperatorLite.NotStartsWith:
                    sb.Append(" NOT LIKE " + QueryLite.DataService.EntityLiteProvider.Concat(parameterName, "'%'"));
                    break;
                case OperatorLite.StartsWith:
                    sb.Append(" LIKE " + QueryLite.DataService.EntityLiteProvider.Concat(parameterName, "'%'"));
                    break;
                case OperatorLite.IsDescendantOf:
                    sb.Append(".IsDescendantOf(" + parameterName + ") = 1");
                    break;
                case OperatorLite.IsChildOf:
                    sb.Append(".GetAncestor(1) = " + parameterName);
                    break;
                case OperatorLite.IsGrandChildOf:
                    sb.Append(".GetAncestor(2) = " + parameterName);
                    break;
                case OperatorLite.HierarchyLevelEquals:
                    sb.Append(".GetLevel() = " + parameterName);
                    break;
                case OperatorLite.STEquals:
                    sb.Append(".STEquals(" + parameterName + ") = 1");
                    break;
                case OperatorLite.STIntersects:
                    sb.Append(".STIntersects(" + parameterName + ") = 1");
                    break;
                case OperatorLite.STDistanceLess:
                case OperatorLite.STDistanceLessOrEquals:
                    string pname = QueryLite.DataService.EntityLiteProvider.ParameterPrefix + "P" + paramIndex.ToString(CultureInfo.InvariantCulture);
                    cmd.Parameters.AddWithValue(pname, condition.Parameter);
                    paramIndex++;
                    sb.Append(".STDistance(" + parameterName + ") " + (condition.Operator == OperatorLite.STDistanceLess ? "< " : "<= ") + pname);
                    break;
                
                default:
                    throw new NotImplementedException("operator " + condition.Operator.ToString() + " not implemented yet");
            }
        }

        private void AddListCondition(DbCommand cmd, ConditionLite condition, StringBuilder sb, ref int paramIndex, ref int indentation, IEnumerable values, IQueryBuilder queryBuilder, PropertyMetadata propertyMetadata, string quotedColumnName)
        {
            if (values != null)
            {
                bool firstValue = true;
                var isNumericField = propertyMetadata.PropertyInfo.PropertyType.IsNumericType();
                var undelyingFieldType = propertyMetadata.PropertyInfo.PropertyType.UndelyingType();
                var isStringField = propertyMetadata.PropertyInfo.PropertyType == typeof(string);
                if ((isNumericField || isStringField) && values.AreThereMoreThan(8))
                {
                    bool firstChunk = true;
                    int valueCount = 0;
                    sb.Append('(');
                    foreach (object v in values)
                    {
                        object value = v;
                        if (valueCount % 1000 == 0)
                        {
                            firstValue = true;
                            if (firstChunk) firstChunk = false;
                            else
                            {
                                sb.Append(") ").Append(condition.Operator == OperatorLite.In ? " OR " : " AND ");
                            }
                            sb.Append(quotedColumnName).Append(condition.Operator == OperatorLite.In ? " IN (" : " NOT IN (");
                        }
                        string valueStr = null;
                        if (value == null)
                        {
                            valueStr = "NULL";
                        }
                        else
                        {
                            if (isNumericField && !value.IsNumeric())
                            {
                                try
                                {
                                    value = Convert.ChangeType(value, undelyingFieldType, CultureInfo.InvariantCulture);
                                }
                                catch( Exception ex)
                                {
                                    throw new ArgumentException($"A non numeric value has been found for {condition.Operator} operator and field {propertyMetadata.PropertyInfo.Name}", ex);
                                }
                            }
                            valueStr = Convert.ToString(value, CultureInfo.InvariantCulture);
                            if (isStringField)
                            {
                                valueStr = "'" + valueStr.Replace("'", "''") + "'";
                            }
                        }
                        if (firstValue) firstValue = false;
                        else sb.Append(", ");
                        sb.Append(valueStr);
                        valueCount++;
                    }
                    sb.Append("))");
                }
                else
                {
                    sb.Append(quotedColumnName).Append(condition.Operator == OperatorLite.In ? " IN (": " NOT IN (");
                    string parameterName;
                    foreach (object value in values)
                    {

                        var param = CreateParameter(propertyMetadata, value, ref paramIndex, out parameterName);
                        cmd.Parameters.Add(param);
                        if (firstValue) firstValue = false;
                        else sb.Append(", ");
                        sb.Append(parameterName);
                    }
                    sb.Append(')');
                }
            }
            else
            {
                sb.Append(quotedColumnName).Append(condition.Operator == OperatorLite.In ? " IN (" : " NOT IN (");
                sb.Append('\n').Append(queryBuilder.GetSelectQuery(cmd, ref paramIndex, ++indentation));
                sb.NewIndentedLine(--indentation);
                sb.Append(')');
            }
            
        }

        public virtual string GetFilter(DbCommand selectCommand, ref int paramIndex, ICollection<ConditionLite> filter, int indentation, bool parenthesis)
        {
            if (selectCommand == null) throw new ArgumentNullException(nameof(selectCommand));
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            StringBuilder sb = new StringBuilder();
            bool firstCondition = true;
            if (parenthesis) sb.Append('(').NewIndentedLine(++indentation);
            foreach (var simpleCondition in filter)
            {
                GenerateFilterForCondition(selectCommand, simpleCondition, sb, ref paramIndex, ref firstCondition, indentation);
            }
            if (parenthesis) sb.NewIndentedLine(--indentation).Append(')');
            return sb.ToString();
        }

        public virtual string GetSort()
        {
            bool firstTime = true;
            StringBuilder sb = new StringBuilder();
            foreach (SortDescriptor sortDescriptor in QueryLite.Sort)
            {
                firstTime = GenerateOrderByForOneField(sb, firstTime, sortDescriptor);
            }
            return sb.ToString();
        }

        public abstract string GetFromClauseContent(DbCommand selectCommand, ref int paramIndex, int indentation);

        protected bool IsFieldValid(string fieldName)
        {
            PropertyMetadata propertyMetadata = null;
            return QueryLite.EntityType.GetEntityMetadata().Properties.TryGetValue(fieldName, out propertyMetadata) && propertyMetadata.SqlField != null;
        }

        protected virtual bool GenerateOrderByForOneField(StringBuilder commandText, bool firstTime, SortDescriptor sortDescriptor)
        {
            if (commandText == null) throw new ArgumentNullException(nameof(commandText));
            if (sortDescriptor == null) throw new ArgumentNullException(nameof(sortDescriptor));
			PropertyMetadata propertyMetadata = null;
            var entityMetadata = QueryLite.EntityType.GetEntityMetadata();
            if (entityMetadata == null) throw new InvalidOperationException("Entity " + QueryLite.EntityType.Name + " has no metadata");
			entityMetadata.Properties.TryGetValue(sortDescriptor.FieldName, out propertyMetadata);

            if (propertyMetadata == null || (propertyMetadata.SqlField == null && !propertyMetadata.IsLocalizedFiled))
            {
                throw new ArgumentException(sortDescriptor.FieldName + " cannot be used in the order by clasuse because it is not a property of " + QueryLite.EntityType.Name + " or it has no metadata and is not a localizable field");
            }

            if (firstTime)
            {
                firstTime = false;
            }
            else
            {
                commandText.Append(", ");
            }

			string fieldName = sortDescriptor.FieldName;

			if ( propertyMetadata.IsLocalizedFiled)
			{
	
				fieldName = CurrentLanguageService.GetSufixedLocalizedFieldName(sortDescriptor.FieldName);

				if (string.IsNullOrEmpty(fieldName))
				{
                    throw new InvalidOperationException("Cannot sort by localized property " + sortDescriptor.FieldName + " because no field name has been found for the current language: " + CurrentLanguageService.CurrentLanguageCode);
				}

                if (!entityMetadata.Properties.TryGetValue(fieldName, out propertyMetadata) || propertyMetadata.SqlField == null || string.IsNullOrEmpty(propertyMetadata.SqlField.ColumnName))
                {
                    throw new InvalidOperationException("Cannot sort by localized property " + sortDescriptor.FieldName + " because no field name has been found for the property: " + fieldName);
                }
			}
            string columnName = propertyMetadata.SqlField.ColumnName;
            commandText.Append(this.QueryLite.DataService.EntityLiteProvider.StartQuote).Append(columnName).Append(this.QueryLite.DataService.EntityLiteProvider.EndQuote).Append(sortDescriptor.SortOrder == SortOrder.Descending ? " DESC" : string.Empty);
            return firstTime;
        }
    }
}
