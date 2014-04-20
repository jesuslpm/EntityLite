using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Extensions
{
    public class PivotColumn
    {
        public string PivotColumnName { get; set; }
        public string ValueColumnName { get; set; }
        public Func<object, string> GetTargetColumnName { get; set; }

        private static readonly Func<object, string> DefaultGetTargetColumnName;

        static PivotColumn()
        {
            DefaultGetTargetColumnName = (x) => x == null ? "Column" : x.ToString();
        }

        public PivotColumn()
        {
            GetTargetColumnName = DefaultGetTargetColumnName;
        }

        public PivotColumn(string pivotColumnName, string valueColumnName)
        {
            this.PivotColumnName = pivotColumnName;
            this.ValueColumnName = valueColumnName;
            this.GetTargetColumnName = DefaultGetTargetColumnName;
        }

        public PivotColumn(string pivotColumName, string valueColumnName, Func<object, string> getTargetColumnName)
        {
            this.PivotColumnName = pivotColumName;
            this.ValueColumnName = valueColumnName;
            this.GetTargetColumnName = getTargetColumnName;
        }
    }
}
