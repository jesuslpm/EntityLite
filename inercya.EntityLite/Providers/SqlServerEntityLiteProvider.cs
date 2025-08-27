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

using inercya.EntityLite.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using inercya.EntityLite.Extensions;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace inercya.EntityLite.Providers
{
    public class SqlServerEntityLiteProvider : EntityLiteProvider
    {
        public const string ProviderName = "System.Data.SqlClient";

        public SqlServerEntityLiteProvider(DataService dataService): base(dataService)
        {
        }
       

        public override string StartQuote
        {
            get { return "["; }
        }

        public override string EndQuote
        {
            get { return "]"; }
        }

        string _defaultSchema;
        public override string DefaultSchema
        {
            get
            {
               if (_defaultSchema == null)
               {
                   _defaultSchema = "dbo";
               }
               return _defaultSchema;
            }
            set
            {
                _defaultSchema = value;
            }
        }


        public override string Concat(params string[] strs)
        {
            return ConcatByOperator(" + ", strs);
        }

		private string GetSelectTopQuery(AbstractQueryBuilder builder, DbCommand selectCommand, int top, ref int paramIndex)
		{
            var commandText = new StringBuilder();
			int indentation = 0;
			if (selectCommand == null) throw new ArgumentNullException(nameof(selectCommand));
			if (commandText == null) throw new ArgumentNullException(nameof(commandText));
			commandText.Indent(indentation);
			var columnList = builder.GetColumnList();
			bool isStar = columnList == "*";
			commandText.Append("SELECT TOP (").Append(top.ToString(CultureInfo.InvariantCulture)).Append(")");
			if (isStar) commandText.Append(" *");
			else commandText.NewIndentedLine(++indentation).Append(columnList);
			if (!isStar) indentation--;
			commandText.NewIndentedLine(indentation).Append("FROM ")
				.NewIndentedLine(++indentation).Append(builder.GetFromClauseContent(selectCommand, ref paramIndex, indentation));
			indentation--;
			bool hasWhereClause = builder.QueryLite.Filter != null && !builder.QueryLite.Filter.IsEmpty();
			if (hasWhereClause)
			{
				commandText.NewIndentedLine(indentation).Append("WHERE");
				commandText.NewIndentedLine(++indentation).Append(builder.GetFilter(selectCommand, ref paramIndex, builder.QueryLite.Filter, indentation, false));
				indentation--;
			}
			bool hasOrderbyClause = builder.QueryLite.Sort != null && builder.QueryLite.Sort.Count > 0;
			if (hasOrderbyClause)
			{
				commandText.NewIndentedLine(indentation).Append("ORDER BY");
				commandText.NewIndentedLine(++indentation).Append(builder.GetSort());
				indentation--;
			}
            return commandText.ToString();
		}

		public override string GetPagedQuery(AbstractQueryBuilder builder, DbCommand selectCommand, ref int paramIndex, int fromRowIndex, int toRowIndex)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (selectCommand == null) throw new ArgumentNullException(nameof(selectCommand));
            if (fromRowIndex == 0) return GetSelectTopQuery(builder, selectCommand, toRowIndex + 1, ref paramIndex);
			StringBuilder commandText = new StringBuilder();
            bool hasOrderbyClause = builder.QueryLite.Sort != null && builder.QueryLite.Sort.Count > 0;
            if (!hasOrderbyClause)
            {
                var metadata = builder.QueryLite.EntityType.GetEntityMetadata();
                if (metadata == null) throw new InvalidOperationException("Entity " + builder.QueryLite.EntityType.Name + " has no metadata");
                foreach (var primaryKey in builder.QueryLite.EntityType.GetEntityMetadata().PrimaryKeyPropertyNames)
                {
                    if (builder.QueryLite.Sort == null) builder.QueryLite.Sort = new List<SortDescriptor>();
                    builder.QueryLite.Sort.Add(new SortDescriptor(primaryKey));
                }
                if (builder.QueryLite.Sort == null || builder.QueryLite.Sort.Count == 0)
                {
                    throw new InvalidOperationException("OrderBy or primary key are requiered for a SQL Server paged query");
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
            string columnList = builder.GetColumnList();
            bool isStar = columnList == "*";
            commandText.Append("SELECT ");
            if (isStar) commandText.Append('*');
            else commandText.NewIndentedLine(1).Append(columnList);
            commandText.Append("\nFROM")
                       .NewIndentedLine(1).Append('(')
                       .NewIndentedLine(2).Append("SELECT")
                       .NewIndentedLine(3).Append(" *, ROW_NUMBER() OVER (ORDER BY ").Append(builder.GetSort()).Append(") AS __RowNumber__")
                       .NewIndentedLine(2).Append("FROM")
                       .NewIndentedLine(3).Append(builder.GetFromClauseContent(selectCommand, ref paramIndex, 3));
            bool hasWhereClause = builder.QueryLite.Filter != null && !builder.QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.NewIndentedLine(2)
                    .Append("WHERE")
                    .NewIndentedLine(3).Append(builder.GetFilter(selectCommand, ref paramIndex, builder.QueryLite.Filter, 3, false));
            }
            commandText.NewIndentedLine(1).Append(") T");
            string fromParameterName;
            IDbDataParameter fromParameter = builder.CreateIn32Parameter(fromRowIndex + 1, ref paramIndex, out fromParameterName);
            selectCommand.Parameters.Add(fromParameter);
            string toParameterName;
            IDbDataParameter toParameter = builder.CreateIn32Parameter(toRowIndex + 1, ref paramIndex, out toParameterName);
            selectCommand.Parameters.Add(toParameter);
            commandText.Append("\nWHERE")
                .NewIndentedLine(1).Append( "__RowNumber__ BETWEEN ")
                .Append(fromParameterName).Append(" AND ").Append(toParameterName);
            commandText.Append("\nORDER BY")
                .NewIndentedLine(1).Append("__RowNumber__");
            builder.SetOptions(commandText);
            return commandText.ToString();
        }

        protected override DbCommand GenerateInsertCommandWithAutogeneratedField(CommandBuilder commandBuilder, object entity, EntityMetadata entityMetadata)
        {
            if (commandBuilder == null) throw new ArgumentNullException(nameof(commandBuilder));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entityMetadata == null) throw new ArgumentNullException(nameof(entityMetadata));
            if (!string.IsNullOrEmpty(entityMetadata.AutoIncrementFieldName)) return base.GenerateInsertCommandWithAutogeneratedField(commandBuilder, entity, entityMetadata);
            if (string.IsNullOrEmpty(entityMetadata.SequenceFieldName)) throw new InvalidOperationException(entity.GetType().Name + " has no autogenerated field");

            var cmd = commandBuilder.DataService.DbProviderFactory.CreateCommand();
            StringBuilder commandText = new StringBuilder();
            commandText.Append(string.Format(CultureInfo.InvariantCulture, "DECLARE {0} bigint = NEXT VALUE FOR {1};", SequenceVariable, entityMetadata.GetFullSequenceName(this.DefaultSchema)));
            commandBuilder.AppendInsertStatement(entity, cmd, commandText);
            commandText.Append(string.Format(CultureInfo.InvariantCulture, ";\nSELECT {0} AS AutogeneratedField;", SequenceVariable));
            cmd.CommandText = commandText.ToString();
            return cmd;
        }

        public override string GetNextValExpression(string fullSequenceName)
        {
            return "NEXT VALUE FOR " + fullSequenceName;
        }

        public override string SequenceVariable
        {
            get
            {
                return "@$equenceGeneratedColumnValue_";
            }
        }


        public override void SetProviderTypeToParameter(IDbDataParameter parameter, int providerType)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            ((SqlParameter)parameter).SqlDbType = (SqlDbType)providerType;
        }

        protected override void AppendGetAutoincrementField(StringBuilder commandText, EntityMetadata entityMetadata)
        {
            if (commandText == null) throw new ArgumentNullException(nameof(commandText));
            commandText.Append(";\nSELECT SCOPE_IDENTITY() AS AutoIncrementField;");
        }
    }
}
