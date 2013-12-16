using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using inercya.EntityLite.Builders;

namespace inercya.EntityLite
{
	public class FunctionQueryLite<TEntity> : AbstractQueryLite<TEntity>, ITableValuedFunctionQueryLite where TEntity : class, new()
    {

        public FunctionQueryLite()
            : base()
        {
			this.CreateQueryBuilder = () => new FunctionQueryBuilder(this, this.FunctionName, this.ParameterValues);
        }


		public FunctionQueryLite(DataService dataAccess, string functionName, params object[] parameterValues) : this()
		{
			this.FunctionName = functionName;
			this.ParameterValues = parameterValues;
			this.DataService = dataAccess;
		}

		public string FunctionName { get; set; }
		public object[] ParameterValues { get; set; }

    }
}
