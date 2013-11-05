using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using inercya.EntityLite.Extensions;

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
       
 
        protected override string GetFromClauseContent(System.Data.Common.DbCommand selectCommand, ref int paramIndex)
        {
            Type entityType = this.QueryLite.EntityType;
            EntityMetadata entityMetadata = entityType.GetEntityMetadata();

            string schema = entityMetadata.SchemaName;
            if (string.IsNullOrEmpty(schema))
            {
                schema = this.QueryLite.DataService.DefaultSchemaName;
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
                tableOrViewName = entityType.Name  + "_" + this.ProjectionName;
            }
            return string.IsNullOrEmpty(schema) ? tableOrViewName : schema + "." + tableOrViewName;           
        }
    }
}
