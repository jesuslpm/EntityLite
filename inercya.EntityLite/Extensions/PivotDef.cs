using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Extensions
{
    public class PivotDef
    {
        public string[] GroupByFields { get; set; }
        public PivotColumn[] PivotColumns { get; set; }
    }
}
