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
using System.Globalization;

namespace inercya.EntityLite.Builders
{
    public class FunctionQueryBuilder : AbstractQueryBuilder
    {

        public new ITableValuedFunctionQueryLite QueryLite
        {
            get
            {
                return (ITableValuedFunctionQueryLite)base.QueryLite;
            }
        }

        public FunctionQueryBuilder(ITableValuedFunctionQueryLite queryLite)
            : base(queryLite)
		{
		}


        public override string GetFromClauseContent(DbCommand selectCommand, ref int paramIndex, int indentation)
        {
            if (selectCommand == null) throw new ArgumentNullException(nameof(selectCommand));
            var sb = new StringBuilder();
            if (this.QueryLite.ParameterValues.Length > 0 || this.QueryLite.DataService.ProviderName != "FirebirdSql.Data.FirebirdClient")
            {
                sb.Append(this.QueryLite.FunctionName).Append('(');
            }
            bool first = true;
            foreach (object parameterValue in this.QueryLite.ParameterValues)
            {
                if (first) first = false;
                else sb.Append(", ");
				string paramName = this.QueryLite.DataService.EntityLiteProvider.ParameterPrefix + "P" + paramIndex.ToString(CultureInfo.InvariantCulture);
                selectCommand.Parameters.AddWithValue(paramName, parameterValue);
                sb.Append(paramName);
				paramIndex++;
            }
            if (this.QueryLite.ParameterValues.Length > 0 || this.QueryLite.DataService.ProviderName != "FirebirdSql.Data.FirebirdClient")
            {
                sb.Append(')');
            }
            sb.Append(' ');
            return sb.ToString();

        }
    }
}
