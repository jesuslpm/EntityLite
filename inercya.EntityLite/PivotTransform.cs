using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public class PivotTransform
    {
        public string PivotColumnName { get; set; }
        public string ValueColumnName { get; set; }
        public Func<object, string> GetPivotedColumnName { get; set; }

        private static readonly Func<object, string> DefaultGetPivotedColumnName;

        static PivotTransform()
        {
            DefaultGetPivotedColumnName = (x) => x == null ? "Column" : x.ToString();
        }

        public PivotTransform()
        {
            GetPivotedColumnName = DefaultGetPivotedColumnName;
        }

        public PivotTransform(string pivotColumnName, string valueColumnName)
        {
            this.PivotColumnName = pivotColumnName;
            this.ValueColumnName = valueColumnName;
            this.GetPivotedColumnName = DefaultGetPivotedColumnName;
        }

        public PivotTransform(string pivotColumName, string valueColumnName, Func<object, string> getPivotedColumnName)
        {
            this.PivotColumnName = pivotColumName;
            this.ValueColumnName = valueColumnName;
            this.GetPivotedColumnName = getPivotedColumnName;
        }
    }
}
