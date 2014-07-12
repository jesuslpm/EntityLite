using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public class PivotDef
    {
        public string[] UnpivotedColumnNames { get; set; }
        public PivotTransform[] PivotTransforms { get; set; }
    }
}
