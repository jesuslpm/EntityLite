using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public class PivotedColumn
    {
        public readonly string PivotColumnName;
        public readonly string ColumnName;
        public readonly object PivotColumnValue;
        public readonly int PivotTransformIndex;

        public PivotedColumn(string pivotColumnName, object pivotColumnValue, int pivotTransformIndex, string columnName)
        {
            this.PivotColumnName = pivotColumnName;
            this.PivotColumnValue = pivotColumnValue;
            this.PivotTransformIndex = pivotTransformIndex;
            this.ColumnName = columnName;
        }
    }
}
