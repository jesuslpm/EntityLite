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
using inercya.EntityLite.Templates;

namespace inercya.EntityLite
{
	public class TemplatedQueryLite<TEntity> : AbstractQueryLite<TEntity>, ITemplatedQueryLite where TEntity : class
    {

        public TemplatedQueryLite()
            : base()
        {
            this.QueryBuilder = new TemplatedQueryBuilder(this);
        }


		public TemplatedQueryLite(DataService dataAccess, ISqlTemplate template) : this()
		{
			this.DataService = dataAccess;
            this.Template = template;
		}


        public ISqlTemplate Template { get; set; }
       
    }
}
