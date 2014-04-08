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
using System.Data.Common;
using inercya.EntityLite.Extensions;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Data.SqlTypes;
using System.Collections;

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
				commandText.Append(")");
			}
		}

        public string GetColumnList()
        {
            if (QueryLite.FieldList == null || QueryLite.FieldList.Count == 0) return "*";

            StringBuilder sb = new StringBuilder();
            bool firstTime = true;
            var properties = QueryLite.EntityType.GetEntityMetadata().Properties;
            string startQuote = QueryLite.DataService.EntityLiteProvider.StartQuote;
            string endQuote = QueryLite.DataService.EntityLiteProvider.EndQuote;
            foreach (var fieldName in QueryLite.FieldList)
            {
                if (firstTime) firstTime = false;
                else sb.Append(", ");
                var propertyMetadata = properties[fieldName];
                if (propertyMetadata.IsLocalizedFiled)
                {
                    propertyMetadata = properties[CurrentLanguageService.GetSufixedLocalizedFieldName(fieldName)];
                }
                sb.Append(startQuote).Append(propertyMetadata.SqlField.ColumnName).Append(endQuote);
            }
            return sb.ToString();
        }

        public void GetSelectQuery(DbCommand selectCommand, ref int paramIndex, StringBuilder commandText)
        {
            commandText.Append("\nSELECT ").Append(GetColumnList()).Append("\nFROM \n    ").Append(GetFromClauseContent(selectCommand, ref paramIndex));
            bool hasWhereClause = QueryLite.Filter != null && !QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.Append("\nWHERE\n    ").Append(GetFilter(selectCommand, ref paramIndex, QueryLite.Filter));
            }
            bool hasOrderbyClause = QueryLite.Sort != null && QueryLite.Sort.Count > 0;
            if (hasOrderbyClause)
            {
                commandText.Append("\nORDER BY\n    ").Append(GetSort());
            }
        }

        public string GetSelectQuery(DbCommand selectCommand, ref int paramIndex)
        {
            StringBuilder commandText = new StringBuilder();
            GetSelectQuery(selectCommand, ref paramIndex, commandText);
			SetOptions(commandText);
            return commandText.ToString();
        }


        string IQueryBuilder.GetSelectQuery(DbCommand selectCommand, ref int paramIndex, int fromRowIndex, int toRowIndex)
        {
            return this.QueryLite.DataService.EntityLiteProvider.GetPagedQuery(this, selectCommand, ref paramIndex, fromRowIndex, toRowIndex);
        }

        string IQueryBuilder.GetCountQuery(DbCommand selectCommand, ref int paramIndex)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("\nSELECT COUNT(*) \nFROM \n    ").Append(GetFromClauseContent(selectCommand, ref paramIndex));
            bool hasWhereClause = QueryLite.Filter != null && !QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.Append("\nWHERE\n    ").Append(GetFilter(selectCommand, ref paramIndex, QueryLite.Filter));
            }
            SetOptions(commandText);
            return commandText.ToString();
        }


        protected IDbDataParameter CreateParameter(PropertyMetadata propertyMetadata, object paramValue, ref int paramIndex)
        {
            string parameterName = QueryLite.DataService.EntityLiteProvider.ParameterPrefix + "P" + paramIndex.ToString();
            IDbDataParameter parameter = QueryLite.DataService.DbProviderFactory.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Size = propertyMetadata.SqlField.Size;

            Type propertyType = propertyMetadata.PropertyInfo.PropertyType.UndelyingType();
            var sqlParam = parameter as SqlParameter;
            if (propertyType.FullName.StartsWith("Microsoft.SqlServer.Types.Sql"))
            {
                if (sqlParam != null)
                {
                    sqlParam.SqlDbType = SqlDbType.Udt;
                    sqlParam.UdtTypeName = propertyType.Name.Substring(3).ToLower();
                }
                else
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
                }
            }
            else
            {

                if (propertyMetadata.SqlField.DbType == DbType.AnsiStringFixedLength)
                {
                    parameter.DbType = DbType.AnsiString;
                }
                else if (propertyMetadata.SqlField.DbType == DbType.StringFixedLength)
                {
                    parameter.DbType = DbType.String;
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
            INullable sqlNullable = paramValue as INullable;
            parameter.Value = (paramValue == null || (sqlNullable != null && sqlNullable.IsNull)) ? DBNull.Value : paramValue;
            paramIndex++;
            return parameter;
        }

        public IDbDataParameter CreateIn32Parameter(int paramValue, ref int paramIndex)
        {
            string parameterName = QueryLite.DataService.EntityLiteProvider.ParameterPrefix + "P" + paramIndex.ToString();
            IDbDataParameter parameter = QueryLite.DataService.DbProviderFactory.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = DbType.Int32;
            parameter.Value = paramValue;
            paramIndex++;
            return parameter;
        }


        protected virtual void GenerateFilterForCondition(DbCommand cmd, ConditionLite condition, StringBuilder sb, ref int paramIndex, ref bool firstCondition)
        {
            if (condition.Filter != null && condition.Filter.IsEmpty()) return;
            if (firstCondition) { ;}
            else if (condition.LogicalOperator == LogicalOperatorLite.And) sb.Append("\n    AND ");
            else if (condition.LogicalOperator == LogicalOperatorLite.Or) sb.Append("\n    OR");
            else throw new NotImplementedException("Logical operator " + condition.LogicalOperator.ToString() + " not implemented");
            firstCondition = false;

            if (condition.Filter != null)
            {
                sb.Append(GetFilter(cmd, ref paramIndex, condition.Filter));
                return;
            }

            IEnumerable values = condition.FieldValue as IEnumerable;
            IQueryBuilder queryBuilder = condition.SubQuery == null ? null : condition.SubQuery.QueryBuilder;
            if (condition.Operator == OperatorLite.In || condition.Operator == OperatorLite.NotIn)
            {
                if (values == null && queryBuilder == null) throw new ArgumentException("The value for In and NotIn operators must be enumerable or a query builder", "expression");
                int count;
                if (values != null)
                {
                    ICollection collection = values as ICollection;
                    Array array = values as Array;
                    if (collection != null) count = collection.Count;
                    else if (array != null) count = array.Length;
                    else count = values.Cast<object>().Count();
                    if (count == 0)
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
            if (!QueryLite.EntityType.GetEntityMetadata().Properties.TryGetValue(fieldName, out propertyMetadata))
            {
                throw new ArgumentException("Field " + condition.FieldName + " cannot be used in a filter because it is not a property of " + QueryLite.EntityType.Name);
            }
			if (propertyMetadata.IsLocalizedFiled)
			{
				fieldName = CurrentLanguageService.GetSufixedLocalizedFieldName(fieldName);

				if (string.IsNullOrEmpty(fieldName))
				{
					throw new InvalidOperationException("Cannot sort by localized property " + fieldName + " because no field name has been found for the current language: " + CurrentLanguageService.CurrentLanguageCode);
				}
				if (!QueryLite.EntityType.GetEntityMetadata().Properties.TryGetValue(fieldName, out propertyMetadata))
				{
					throw new ArgumentException("Field " + fieldName + " cannot be used in a filter because it is not a property of " + QueryLite.EntityType.Name);
				}
			}
			if (propertyMetadata.SqlField == null)
			{
				throw new ArgumentException("Field " + fieldName + " cannot be used in a filter because it has no metadata");
			}

			sb.Append(this.QueryLite.DataService.EntityLiteProvider.StartQuote).Append(propertyMetadata.SqlField.ColumnName).Append(this.QueryLite.DataService.EntityLiteProvider.EndQuote);
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
                if (condition.Operator == OperatorLite.In) sb.Append(" IN (");
                else sb.Append(" NOT IN (");
                if (values != null)
                {
                    bool firstValue = true;
                    foreach (object value in values)
                    {
                        var param = CreateParameter(propertyMetadata, value, ref paramIndex);
                        cmd.Parameters.Add(param);
                        if (firstValue) firstValue = false;
                        else sb.Append(", ");
                        sb.Append(param.ParameterName);
                    }
                }
                else
                {
                    sb.Append(queryBuilder.GetSelectQuery(cmd, ref paramIndex));
                }
                sb.Append(")");
                return;
            }

            var parameter = CreateParameter(propertyMetadata, condition.FieldValue, ref paramIndex);
            cmd.Parameters.Add(parameter);

            switch (condition.Operator)
            {
                case OperatorLite.Contains:
                    sb.Append(" LIKE " + QueryLite.DataService.EntityLiteProvider.Concat("'%'", parameter.ParameterName, "'%'"));
                    break;
                case OperatorLite.Equals:
                    sb.Append(" = ").Append(parameter.ParameterName);
                    break;
                case OperatorLite.Greater:
                    sb.Append(" > ").Append(parameter.ParameterName);
                    break;
                case OperatorLite.GreaterOrEquals:
                    sb.Append(" >= ").Append(parameter.ParameterName);
                    break;
                case OperatorLite.Less:
                    sb.Append(" < ").Append(parameter.ParameterName);
                    break;
                case OperatorLite.LessOrEquals:
                    sb.Append(" <= ").Append(parameter.ParameterName);
                    break;
                case OperatorLite.NotContains:
                    sb.Append(" NOT LIKE " + QueryLite.DataService.EntityLiteProvider.Concat("'%'", parameter.ParameterName, "'%'"));
                    break;
                case OperatorLite.NotEquals:
                    sb.Append(" <> ").Append(parameter.ParameterName);
                    break;
                case OperatorLite.NotStartsWith:
                    sb.Append(" NOT LIKE " + QueryLite.DataService.EntityLiteProvider.Concat(parameter.ParameterName, "'%'"));
                    break;
                case OperatorLite.StartsWith:
                    sb.Append(" LIKE " + QueryLite.DataService.EntityLiteProvider.Concat(parameter.ParameterName, "'%'"));
                    break;
                case OperatorLite.IsDescendantOf:
                    sb.Append(".IsDescendantOf(" + parameter.ParameterName + ") = 1");
                    break;
                case OperatorLite.IsChildOf:
                    sb.Append(".GetAncestor(1) = " + parameter.ParameterName);
                    break;
                case OperatorLite.IsGrandChildOf:
                    sb.Append(".GetAncestor(2) = " + parameter.ParameterName);
                    break;
                case OperatorLite.HierarchyLevelEquals:
                    sb.Append(".GetLevel() = " + parameter.ParameterName);
                    break;
                case OperatorLite.STEquals:
                    sb.Append(".STEquals(" + parameter.ParameterName + ") = 1");
                    break;
                case OperatorLite.STIntersects:
                    sb.Append(".STIntersects(" + parameter.ParameterName + ") = 1");
                    break;
                case OperatorLite.STDistanceLess:
                case OperatorLite.STDistanceLessOrEquals:
                    string parameterName = QueryLite.DataService.EntityLiteProvider.ParameterPrefix + "P" + paramIndex.ToString();
                    cmd.Parameters.AddWithValue(parameterName, condition.Parameter);
                    paramIndex++;
                    sb.Append(".STDistance(" + parameter.ParameterName + ") " + (condition.Operator == OperatorLite.STDistanceLess ? "< " : "<= ") + parameterName);
                    break;
                default:
                    throw new NotImplementedException("operator " + condition.Operator.ToString() + " not implemented yet");
            }
        }

        public virtual string GetFilter(DbCommand selectCommand, ref int paramIndex, ICollection<ConditionLite> filter)
        {
            StringBuilder sb = new StringBuilder();
            bool firstCondition = true;
            sb.Append('(');
            foreach (var simpleCondition in filter)
            {
                GenerateFilterForCondition(selectCommand, simpleCondition, sb, ref paramIndex, ref firstCondition);
            }
            sb.Append(')');
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

        public abstract string GetFromClauseContent(DbCommand selectCommand, ref int paramIndex);

        protected bool IsFieldValid(string fieldName)
        {
            PropertyMetadata propertyMetadata = null;
            return QueryLite.EntityType.GetEntityMetadata().Properties.TryGetValue(fieldName, out propertyMetadata) && propertyMetadata.SqlField != null;
        }

        protected virtual bool GenerateOrderByForOneField(StringBuilder commandText, bool firstTime, SortDescriptor sortDescriptor)
        {
			PropertyMetadata propertyMetadata = null;
            var entityMetadata = QueryLite.EntityType.GetEntityMetadata();
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
                commandText.Append(",\n    ");
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
