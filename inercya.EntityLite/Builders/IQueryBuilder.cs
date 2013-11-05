using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace inercya.EntityLite.Builders
{
    public interface IQueryBuilder
    {
        string GetSelectQuery(DbCommand selectCommand, ref int paramIndex);
        string GetSelectQuery(DbCommand selectCommand, ref int paramIndex, int fromRowIndex, int toRowIndex);
        string GetCountQuery(DbCommand selectCommand, ref int paramIndex);
        IQueryLite QueryLite { get; set; }
    }
}
