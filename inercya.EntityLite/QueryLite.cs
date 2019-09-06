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
            this.QueryBuilder =  new ProjectedQueryBuilder(this);           
        }
        public string ProjectionName { get; set; }

        public QueryLite(string projectionName, DataService dataAccess) : this()
        {
            this.ProjectionName = projectionName;
            this.DataService = dataAccess;
            this.FieldList = new List<string>();
        }
    }

    [Serializable]
    public class TableOrViewQueryLite<TEntity> : AbstractQueryLite<TEntity>, ITableOrViewQueryLite where TEntity : class, new()
    {


        public TableOrViewQueryLite() : base()
        {
            this.QueryBuilder = new TableOrViewQueryBuilder(this);
        }
        public string FullyQualifiedTableOrViewName { get; set; }

        public TableOrViewQueryLite(string fullyQualifiedTableOrViewName, DataService dataAccess) : this()
        {
            this.FullyQualifiedTableOrViewName = fullyQualifiedTableOrViewName;
            this.DataService = dataAccess;
            this.FieldList = new List<string>();
        }
    }
}
