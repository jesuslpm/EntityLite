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

        public TemplatedQueryBuilder(ITemplatedQueryLite queryLite)
		{
			this.QueryLite = queryLite;
		}

        public override string GetFromClauseContent(DbCommand selectCommand, ref int paramIndex)
        {
            ISqlTemplate template = ((ITemplatedQueryLite)this.QueryLite).Template;
            string parameterPrefix = this.QueryLite.DataService.EntityLiteProvider.ParameterPrefix;
            template.AddParametersToCommand(selectCommand, parameterPrefix);
            return "(\n" + template.GetSql(parameterPrefix) + "\n) TQ";
        }
    }
}
