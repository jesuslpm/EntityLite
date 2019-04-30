using inercya.EntityLite.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebirdFx
{

    public class CommandTemplateResult
    {
        public int AffectedRows { get; set; }
        public int ErrorCount { get; set; }
    }

    public partial class CommandTemplate : ISqlTemplate
    {
        [DbParameter(DbType = System.Data.DbType.Int32)]
        public int CustomerNumber { get; set; }
    }
}
