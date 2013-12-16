using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using inercya.EntityLite.Extensions;
using System.Data.Common;

namespace inercya.EntityLite.Builders
{
    public class FunctionQueryBuilder : AbstractQueryBuilder
    {

		public FunctionQueryBuilder()
		{
		}

		public FunctionQueryBuilder(IQueryLite queryLite, string functionName, params object[] parameterValues)
		{
			this.QueryLite = queryLite;
			this.FunctionName = functionName;
			this.ParameterValues = parameterValues;
		}

        public string FunctionName { get; set; }

        public object[] ParameterValues { get; set; }

        public override string GetFromClauseContent(DbCommand selectCommand, ref int paramIndex)
        {
            var sb = new StringBuilder();
            sb.Append(" ").Append(this.FunctionName).Append("(");
            bool first = true;
            foreach (object parameterValue in this.ParameterValues)
            {
                if (first) first = false;
                else sb.Append(", ");
				string paramName = this.QueryLite.DataService.EntityLiteProvider.ParameterPrefix + "P" + paramIndex.ToString();
                selectCommand.Parameters.AddWithValue(paramName, parameterValue);
                sb.Append(paramName);
				paramIndex++;
            }
            sb.Append(") ");
            return sb.ToString();

        }
    }
}
