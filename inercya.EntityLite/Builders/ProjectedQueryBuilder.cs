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
    public class ProjectedQueryBuilder : AbstractQueryBuilder
    {

        public ProjectedQueryBuilder()
        {
        }

        public ProjectedQueryBuilder(IQueryLite queryLite, string projectionName)
        {
            if (queryLite == null) throw new ArgumentNullException("queryLite");
            if (string.IsNullOrEmpty(projectionName)) throw new ArgumentNullException("projectionName");
            this.QueryLite = queryLite;
            this.ProjectionName = projectionName;
        }


        public string ProjectionName { get; set; }
       
 
        public override string GetFromClauseContent(DbCommand selectCommand, ref int paramIndex)
        {
            Type entityType = this.QueryLite.EntityType;
            EntityMetadata entityMetadata = entityType.GetEntityMetadata();

            string schema = entityMetadata.SchemaName;
            if (string.IsNullOrEmpty(schema))
            {
                schema = this.QueryLite.DataService.EntityLiteProvider.DefaultSchema;
            }
            string tableOrViewName;

            if (this.ProjectionName == Projection.BaseTable.GetProjectionName())
            {
                if (string.IsNullOrEmpty(entityMetadata.BaseTableName))
                {
                    throw new InvalidOperationException("Base table not set on entity " + entityType.Name);
                }
                tableOrViewName = entityMetadata.BaseTableName;
            }
            else
            {
                tableOrViewName = (entityType.Name  + "_" + this.ProjectionName).Transform(QueryLite.DataService.EntityNameToEntityViewTransform);
            }
            string startQuote =  this.QueryLite.DataService.EntityLiteProvider.StartQuote;
            string endQuote =  this.QueryLite.DataService.EntityLiteProvider.EndQuote;
            return string.IsNullOrEmpty(schema) ? ( startQuote + tableOrViewName + endQuote): startQuote + schema + endQuote + "." + startQuote + tableOrViewName + endQuote;           
        }
    }
}
