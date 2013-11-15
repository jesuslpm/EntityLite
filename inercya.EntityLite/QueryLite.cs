using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Data.SqlTypes;
using NLog;
using inercya.EntityLite.Extensions;
using inercya.EntityLite.Builders;

namespace inercya.EntityLite
{

    [Serializable]
    public class QueryLite<TEntity> : AbstractQueryLite<TEntity>, IProjectedQueryLite where TEntity : class, new()
    {
        public QueryLite(Projection projection, DataService dataAccess) : this(projection.GetProjectionName(), dataAccess)
        {  
        }

        public QueryLite() : base()
        {
            this.CreateQueryBuilder = () => new ProjectedQueryBuilder(this, this.ProjectionName);
              
        }
        public string ProjectionName { get; set; }

        public QueryLite(string projectionName, DataService dataAccess) : this()
        {
            this.ProjectionName = projectionName;
            this.DataService = dataAccess;
            this.FieldList = new List<string>();
        }
    }
}
