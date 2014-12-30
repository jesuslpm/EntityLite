using inercya.EntityLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samples.Entities
{
    public partial class ItemRepository
    {
        public void Truncate()
        {
            var template = new Templates.TruncateItemsTableCommandTemplate();
            var cmd = new TemplatedCommand(this.DataService, template);
            cmd.ExecuteNonQuery();
        }
    }
}
