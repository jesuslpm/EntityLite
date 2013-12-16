using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using inercya.EntityLite.Builders;
using inercya.EntityLite.Templates;

namespace inercya.EntityLite
{
	public class TemplatedQueryLite<TEntity> : AbstractQueryLite<TEntity>, ITemplatedQueryLite where TEntity : class, new()
    {

        public TemplatedQueryLite()
            : base()
        {
			this.CreateQueryBuilder = () => new TemplatedQueryBuilder(this);
        }


		public TemplatedQueryLite(DataService dataAccess, ISqlTemplate template) : this()
		{
			this.DataService = dataAccess;
            this.Template = template;
		}


        public ISqlTemplate Template { get; set; }
       
    }
}
