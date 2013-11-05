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

        public virtual IQueryLite QueryLite { get; set; }

		private void SetOptions(StringBuilder commandText)
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

        public string GetSelectQuery(DbCommand selectCommand, ref int paramIndex)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("\nSELECT ").Append(QueryLite.FieldList).Append("\nFROM \n\t").Append(GetFromClauseContent(selectCommand, ref paramIndex));
            bool hasWhereClause = QueryLite.Filter != null && !QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.Append("\nWHERE\n\t").Append(GetFilter(selectCommand, ref paramIndex, QueryLite.Filter));
            }
            bool hasOrderbyClause = QueryLite.Sort != null && QueryLite.Sort.Count > 0;
            if (hasOrderbyClause)
            {
                commandText.Append("\nORDER BY\n\t").Append(GetSort());
            }
			SetOptions(commandText);
            return commandText.ToString();
        }

        string IQueryBuilder.GetSelectQuery(DbCommand selectCommand, ref int paramIndex, int fromRowIndex, int toRowIndex)
        {
            StringBuilder commandText = new StringBuilder();
            bool hasOrderbyClause = QueryLite.Sort != null && QueryLite.Sort.Count > 0;
            if (!hasOrderbyClause)
            {
                foreach (var primaryKey in QueryLite.EntityType.GetEntityMetadata().PrimaryKeyPropertyNames)
                {
					if (QueryLite.Sort == null) QueryLite.Sort = new List<SortDescriptor>();
                    QueryLite.Sort.Add(new SortDescriptor(primaryKey));
                }
				if (QueryLite.Sort == null || QueryLite.Sort.Count == 0)
				{
					throw new InvalidOperationException("OrderBy or primary key are requiered for a paged query");
				}
            }

            /*
SELECT *
FROM (
SELECT  *, ROW_NUMBER() OVER (ORDER BY CompanyName) AS __RowNumber__,
FROM dbo.Customers C
WHERE C.Country = 'USA'
) T
WHERE __RowNumber__ BETWEEN 1 AND 5
ORDER BY __RowNumber__
 */

            commandText.Append("\nSELECT ").Append(QueryLite.FieldList).Append("\n")
                       .Append("FROM (\n")
                       .Append("SELECT *, ROW_NUMBER() OVER (ORDER BY ").Append(this.GetSort()).Append(") AS __RowNumber__\n")
                       .Append("FROM ").Append(GetFromClauseContent(selectCommand, ref paramIndex)).Append("\n");
            bool hasWhereClause = QueryLite.Filter != null && !QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.Append("\nWHERE\n\t").Append(GetFilter(selectCommand, ref paramIndex, QueryLite.Filter));
            }
            commandText.Append("\n) T\n");
            IDbDataParameter fromParameter = CreateIn32Parameter(fromRowIndex + 1, ref paramIndex);
            selectCommand.Parameters.Add(fromParameter);
            IDbDataParameter toParameter = CreateIn32Parameter(toRowIndex + 1, ref paramIndex);
            selectCommand.Parameters.Add(toParameter);
            commandText.Append("WHERE __RowNumber__ BETWEEN ").Append(fromParameter.ParameterName).Append(" AND ").Append(toParameter.ParameterName);
            commandText.Append("\nORDER BY __RowNumber__");

			SetOptions(commandText);
            return commandText.ToString();
        }

        string IQueryBuilder.GetCountQuery(DbCommand selectCommand, ref int paramIndex)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("\nSELECT COUNT(*) \nFROM \n\t").Append(GetFromClauseContent(selectCommand, ref paramIndex));
            bool hasWhereClause = QueryLite.Filter != null && !QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.Append("\nWHERE\n\t").Append(GetFilter(selectCommand, ref paramIndex, QueryLite.Filter));
            }
            SetOptions(commandText);
            return commandText.ToString();
        }


        protected IDbDataParameter CreateParameter(PropertyMetadata propertyMetadata, object paramValue, ref int paramIndex)
        {
            string parameterName = QueryLite.DataService.ParameterPrefix + "P" + paramIndex.ToString();
            IDbDataParameter parameter = QueryLite.DataService.ProviderFactory.CreateParameter();
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

        protected IDbDataParameter CreateIn32Parameter(int paramValue, ref int paramIndex)
        {
            string parameterName = QueryLite.DataService.ParameterPrefix + "P" + paramIndex.ToString();
            IDbDataParameter parameter = QueryLite.DataService.ProviderFactory.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = DbType.Int32;
            parameter.Value = paramValue;
            paramIndex++;
            return parameter;
        }

        protected virtual void GenerateFilterForSimpleCondition(DbCommand cmd, SimpleConditionLite simpleCondition, StringBuilder sb, ref int paramIndex, ref bool firstCondition)
        {
            if (simpleCondition.Filter != null && simpleCondition.Filter.IsEmpty()) return;
            if (firstCondition) { ;}
            else if (simpleCondition.LogicalOperator == LogicalOperatorLite.And) sb.Append("\n\tAND ");
            else if (simpleCondition.LogicalOperator == LogicalOperatorLite.Or) sb.Append("\n\tOR");
            else throw new NotImplementedException("Logical operator " + simpleCondition.LogicalOperator.ToString() + " not implemented");
            firstCondition = false;

            if (simpleCondition.Filter != null)
            {
                sb.Append(GetFilter(cmd, ref paramIndex, simpleCondition.Filter));
                return;
            }

            IEnumerable values = simpleCondition.FieldValue as IEnumerable;
            IQueryBuilder queryBuilder = simpleCondition.SubQuery == null ? null : simpleCondition.SubQuery.CreateQueryBuilder();
            if (simpleCondition.Operator == OperatorLite.In || simpleCondition.Operator == OperatorLite.NotIn)
            {
                if (values == null && queryBuilder == null) throw new ArgumentException("The value for In and NotIn operators must be enumerable or a query builder", "expression");
                int count;
                if (values != null)
                {
                    ICollection collection = values as ICollection;
                    IList list = values as IList;
                    Array array = values as Array;
                    if (list != null) count = list.Count;
                    else if (collection != null) count = collection.Count;
                    else if (array != null) count = array.Length;
                    else count = values.Cast<object>().Count();
                    if (count == 0)
                    {
                        if (simpleCondition.Operator == OperatorLite.In)
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
			string fieldName = simpleCondition.FieldName;
            if (!QueryLite.EntityType.GetEntityMetadata().Properties.TryGetValue(fieldName, out propertyMetadata))
            {
                throw new ArgumentException("Field " + simpleCondition.FieldName + " cannot be used in a filter because it is not a property of " + QueryLite.EntityType.Name);
            }
			if (propertyMetadata.IsLocalizedFiled)
			{
				if (ContextLite.LocalizationContext == null)
				{
					throw new InvalidOperationException("Cannot sort by localized property " + fieldName + " beacause the language context is not set");
				}
				fieldName = ContextLite.GetSufixedLocalizedFieldName(fieldName);

				if (string.IsNullOrEmpty(fieldName))
				{
					throw new InvalidOperationException("Cannot sort by localized property " + fieldName + " because no field name has been found for the current language: " + ContextLite.LocalizationContext.CurrentLanguageCode);
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

			sb.Append("[").Append(fieldName).Append("]");
            if (simpleCondition.Operator == OperatorLite.IsNull)
            {
                sb.Append(" IS NULL");
                return;
            }
            if (simpleCondition.Operator == OperatorLite.IsNotNull)
            {
                sb.Append(" IS NOT NULL");
                return;
            }

            if (simpleCondition.Operator == OperatorLite.In || simpleCondition.Operator == OperatorLite.NotIn)
            {
                if (simpleCondition.Operator == OperatorLite.In) sb.Append(" IN (");
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

            var parameter = CreateParameter(propertyMetadata, simpleCondition.FieldValue, ref paramIndex);
            cmd.Parameters.Add(parameter);

            switch (simpleCondition.Operator)
            {
                case OperatorLite.Contains:
                    sb.Append(" LIKE '%' +").Append(parameter.ParameterName).Append("+'%'");
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
                    sb.Append(" NOT LIKE '%' +").Append(parameter.ParameterName).Append("+'%'");
                    break;
                case OperatorLite.NotEquals:
                    sb.Append(" <> ").Append(parameter.ParameterName);
                    break;
                case OperatorLite.NotStartsWith:
                    sb.Append(" NOT LIKE ").Append(parameter.ParameterName).Append("+'%'");
                    break;
                case OperatorLite.StartsWith:
                    sb.Append(" LIKE ").Append(parameter.ParameterName).Append("+'%'");
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
                    string parameterName = QueryLite.DataService.ParameterPrefix + "P" + paramIndex.ToString();
                    cmd.Parameters.AddWithValue(parameterName, simpleCondition.Parameter);
                    paramIndex++;
                    sb.Append(".STDistance(" + parameter.ParameterName + ") " + (simpleCondition.Operator == OperatorLite.STDistanceLess ? "< " : "<= ") + parameterName);
                    break;
                default:
                    throw new NotImplementedException("operator " + simpleCondition.Operator.ToString() + " not implemented yet");

            }

        }



        protected virtual string GetFilter(DbCommand selectCommand, ref int paramIndex, ICollection<SimpleConditionLite> filter)
        {
            StringBuilder sb = new StringBuilder();
            bool firstCondition = true;
            sb.Append('(');
            foreach (var simpleCondition in filter)
            {
                GenerateFilterForSimpleCondition(selectCommand, simpleCondition, sb, ref paramIndex, ref firstCondition);
            }
            sb.Append(')');
            return sb.ToString();
        }

        protected virtual string GetSort()
        {
            bool firstTime = true;
            StringBuilder sb = new StringBuilder();
            foreach (SortDescriptor sortDescriptor in QueryLite.Sort)
            {
                firstTime = GenerateOrderByForOneField(sb, firstTime, sortDescriptor);
            }
            return sb.ToString();
        }

        protected abstract string GetFromClauseContent(DbCommand selectCommand, ref int paramIndex);

        protected bool IsFieldValid(string fieldName)
        {
            PropertyMetadata propertyMetadata = null;
            return QueryLite.EntityType.GetEntityMetadata().Properties.TryGetValue(fieldName, out propertyMetadata) && propertyMetadata.SqlField != null;
        }

        protected virtual bool GenerateOrderByForOneField(StringBuilder commandText, bool firstTime, SortDescriptor sortDescriptor)
        {
			PropertyMetadata propertyMetadata = null;
			QueryLite.EntityType.GetEntityMetadata().Properties.TryGetValue(sortDescriptor.FieldName, out propertyMetadata);

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
                commandText.Append(",\n\t");
            }

			string fieldName = sortDescriptor.FieldName;

			if ( propertyMetadata.IsLocalizedFiled)
			{
				if (ContextLite.LocalizationContext == null)
				{
					throw new InvalidOperationException("Cannot sort by localized property " + fieldName + " beacause the language context is not set");
				}
				fieldName = ContextLite.GetSufixedLocalizedFieldName(sortDescriptor.FieldName);

				if (string.IsNullOrEmpty(fieldName))
				{
					throw new InvalidOperationException("Cannot sort by localized property " + fieldName + " because no field name has been found for the current language: " + ContextLite.LocalizationContext.CurrentLanguageCode);
				}
			}
			commandText.Append(fieldName).Append(sortDescriptor.SortOrder == SortOrder.Descending ? " DESC" : string.Empty);
            return firstTime;
        }
    }
}
