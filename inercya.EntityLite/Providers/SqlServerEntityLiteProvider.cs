using inercya.EntityLite.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using inercya.EntityLite.Extensions;
using System.Data;

namespace inercya.EntityLite.Providers
{
    public class SqlServerEntityLiteProvider : EntityLiteProvider
    {
        public const string ProviderName = "System.Data.SqlClient";
        private readonly DataService DataService;

        public SqlServerEntityLiteProvider(DataService dataService)
        {
            this.DataService = dataService;
            if (DataService.ProviderName != ProviderName)
            {
                throw new InvalidOperationException(this.GetType().Name + " is for " + ProviderName + ". Not for " + DataService.ProviderName);
            }
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

        public override string GetPagedQuery(AbstractQueryBuilder builder, DbCommand selectCommand, ref int paramIndex, int fromRowIndex, int toRowIndex)
        {
            StringBuilder commandText = new StringBuilder();
            bool hasOrderbyClause = builder.QueryLite.Sort != null && builder.QueryLite.Sort.Count > 0;
            if (!hasOrderbyClause)
            {
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

            commandText.Append("\nSELECT ").Append(builder.GetColumnList()).Append("\n")
                       .Append("FROM (\n")
                       .Append("SELECT *, ROW_NUMBER() OVER (ORDER BY ").Append(builder.GetSort()).Append(") AS __RowNumber__\n")
                       .Append("FROM ").Append(builder.GetFromClauseContent(selectCommand, ref paramIndex)).Append("\n");
            bool hasWhereClause = builder.QueryLite.Filter != null && !builder.QueryLite.Filter.IsEmpty();
            if (hasWhereClause)
            {
                commandText.Append("\nWHERE\n    ").Append(builder.GetFilter(selectCommand, ref paramIndex, builder.QueryLite.Filter));
            }
            commandText.Append("\n) T\n");
            IDbDataParameter fromParameter = builder.CreateIn32Parameter(fromRowIndex + 1, ref paramIndex);
            selectCommand.Parameters.Add(fromParameter);
            IDbDataParameter toParameter = builder.CreateIn32Parameter(toRowIndex + 1, ref paramIndex);
            selectCommand.Parameters.Add(toParameter);
            commandText.Append("WHERE __RowNumber__ BETWEEN ").Append(fromParameter.ParameterName).Append(" AND ").Append(toParameter.ParameterName);
            commandText.Append("\nORDER BY __RowNumber__;");

            builder.SetOptions(commandText);
            return commandText.ToString();
        }

        protected override void AppendGetAutoincrementField(StringBuilder commandText, EntityMetadata entityMetadata)
        {
            commandText.Append(";\nSELECT SCOPE_IDENTITY() AS AutoIncrementField;");
        }
    }
}
