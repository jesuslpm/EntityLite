using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    [Flags]
    public enum FieldsOption
    {
        None = 0,
        IncludePrimaryKey = 1,
        IncludeEntityRowVersion = 2,
        IncludeBoth = 3
    }
}
