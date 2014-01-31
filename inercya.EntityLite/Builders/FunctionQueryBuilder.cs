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
