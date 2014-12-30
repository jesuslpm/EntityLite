using inercya.EntityLite.Templates;
using Samples.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Samples.Entities.Templates
{
    public partial class TruncateItemsTableCommandTemplate : ISqlTemplate
    {

        public string TransformText()
        {
            return "TRUNCATE TABLE Items;";
        }
    }
}
