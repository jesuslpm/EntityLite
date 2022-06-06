using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public class PivotedColumn
    {
        public string PivotColumnName { get; private set; }
        public string ColumnName { get; private set; }
        public object PivotColumnValue { get; private set; }
        public int PivotTransformIndex { get; private set; }

        public PivotedColumn(string pivotColumnName, object pivotColumnValue, int pivotTransformIndex, string columnName)
        {
            this.PivotColumnName = pivotColumnName;
            this.PivotColumnValue = pivotColumnValue;
            this.PivotTransformIndex = pivotTransformIndex;
            this.ColumnName = columnName;
        }
    }
}
