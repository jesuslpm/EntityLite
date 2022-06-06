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
using System.Data.Common;
using System.Linq;
using System.Text;
using inercya.EntityLite;
using inercya.EntityLite.Extensions;
using inercya.EntityLite.Templates;

namespace inercya.EntityLite.Builders
{
    class TemplatedQueryBuilder: AbstractQueryBuilder
    {

        public TemplatedQueryBuilder(ITemplatedQueryLite queryLite) : base(queryLite)
		{
		}

        private string sql;

        public override string GetFromClauseContent(DbCommand selectCommand, ref int paramIndex, int indentation)
        {
            ISqlTemplate template = ((ITemplatedQueryLite)this.QueryLite).Template;
            string parameterPrefix = this.QueryLite.DataService.EntityLiteProvider.ParameterPrefix;
            template.AddParametersToCommand(selectCommand, this.QueryLite.DataService);
            if (string.IsNullOrEmpty(sql))
            {
                var sb = new StringBuilder();
                sb.NewIndentedLine(++indentation);
                var indentedLine = sb.ToString();
                sql = template.GetSql(parameterPrefix).Replace("\r\n", indentedLine);
                sb = new StringBuilder();
                sb.Append('(').Append('\n').Append(sql).NewIndentedLine(--indentation).Append(") TQ");
                sql = sb.ToString();
            }
            return sql;
        }
    }
}
