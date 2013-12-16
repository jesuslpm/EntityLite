using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using inercya.EntityLite.Templates;
using System.Data;

namespace Samples
{
    public partial class SalesQueryTemplate : ISqlTemplate
    {
        public string Grouping { get; set; }

        [DbParameter(DbType= DbType.Int32)]
        public int? EmployeeId { get; set; }
    }
}
