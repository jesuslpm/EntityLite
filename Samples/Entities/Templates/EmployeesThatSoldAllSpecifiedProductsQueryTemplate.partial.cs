using inercya.EntityLite.Templates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Samples.Entities.Templates
{
    public partial class EmployeesThatSoldAllSpecifiedProductsQueryTemplate : ISqlTemplate
    {
        [DbParameterSerie(DbType=DbType.Int32)]
        public IEnumerable<int> ProductIds { get; set; }

        private string GetProductIdCommaSeparatedParameterList()
        {
            return string.Join(", ", ProductIds.Select((x, i) => "($(ProductIds" + i.ToString() + "))"));
        }

    }
}
