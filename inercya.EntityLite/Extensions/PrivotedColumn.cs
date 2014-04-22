using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Extensions
{
    public class PivotedColumn
    {
        public readonly string PivotColumnName;
        public readonly string ColumnName;
        public readonly object PivotColumnValue;
        public readonly int PivotColumnIndex;

        public PivotedColumn(string pivotColumnName, object pivotColumnValue, int pivotColumnIndex, string columnName)
        {
            this.PivotColumnName = pivotColumnName;
            this.PivotColumnValue = pivotColumnValue;
            this.PivotColumnIndex = pivotColumnIndex;
            this.ColumnName = columnName;
        }
    }
}
