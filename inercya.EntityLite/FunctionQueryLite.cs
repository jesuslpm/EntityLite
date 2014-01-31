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
