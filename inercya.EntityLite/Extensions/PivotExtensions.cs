using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Extensions
{
    public static class PivotExtensions
    {
        public static DataTable Pivot(this IDataReader reader, PivotDef pivotDef, Comparison<PivotedColumn> pivotedColumnComparison)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (pivotDef == null) throw new ArgumentNullException("pivotDef");
            if (pivotedColumnComparison == null) throw new ArgumentNullException("pivotedColumnComparison");
            if (pivotDef.GroupByFields == null) throw new ArgumentException("GroupByFields must not be null");
            if (pivotDef.PivotColumns == null) throw new ArgumentException("PivotColumns must not be null");
            if (pivotDef.PivotColumns.Any(x => x == null)) throw new ArgumentException("All PivotColums mut be not null");
            var builder = new SimplePivotBuilder(pivotDef, reader, pivotedColumnComparison);
            return builder.Build();
        }

        public static DataTable Pivot(this IDataReader reader, PivotDef pivotDef)
        {
            var builder = new SimplePivotBuilder(pivotDef, reader, DefaultPivotedColumnComparison);
            return builder.Build();
        }  

        internal static int DefaultPivotedColumnComparison(PivotedColumn c1, PivotedColumn c2)
        {
            int indexComparisonResult = c1.PivotColumnIndex.CompareTo(c2.PivotColumnIndex);
            if (indexComparisonResult != 0) return indexComparisonResult;
            var isC1Null = c1.PivotColumnValue == null || c1.PivotColumnValue == DBNull.Value;
            var isC2Null = c2.PivotColumnValue == null || c2.PivotColumnValue == DBNull.Value;

            if (isC1Null && isC2Null) return 0;
            if (isC1Null) return -1;
            if (isC2Null) return 1;

            var comparable = c1.PivotColumnValue as IComparable;
            if (comparable != null) return comparable.CompareTo(c2.PivotColumnValue);
            return (c1.ColumnName.CompareTo(c2.ColumnName));
        }

   

        


        private class SimplePivotBuilder
        {

            private readonly Comparison<PivotedColumn> PivotedColumnComparison;
            private readonly PivotDef PivotDef;
            private readonly IDataReader Reader;

            private Record PreviousKey;
            private Record CurrentKey;
            private DataTable TargetTable;
            private DataRow CurrentRow;

            private int[] KeyFieldsMappings;
            private int[] PivotColumnsMappings;
            private int[] ValueColumnsMappings;

            private readonly List<PivotedColumn> PivotedColumns = new List<PivotedColumn>();

            public SimplePivotBuilder(PivotDef pivotDef, IDataReader reader, Comparison<PivotedColumn> pivotedColumnComparison)
            {
                this.PivotedColumnComparison = pivotedColumnComparison;
                this.PivotDef = pivotDef;
                this.Reader = reader;
            }

            private void ReadCurrentKey()
            {
                if (CurrentKey == null)
                {
                    CurrentKey = new Record(this.PivotDef.GroupByFields.Length);
                }
                for (int i = 0; i < this.PivotDef.GroupByFields.Length; i++)
                {
                    this.CurrentKey[i] = Reader[this.KeyFieldsMappings[i]];
                }
            }

            private void SwapPreviousAndCurrentKeys()
            {
                Record aux = PreviousKey;
                PreviousKey = CurrentKey;
                CurrentKey = aux;
            }

            private int GetFieldIndex(string fieldName)
            {
                int fieldIndex = 0;
                if (!readerFields.TryGetValue(fieldName, out fieldIndex) && !pascalReaderFields.TryGetValue(fieldName, out fieldIndex))
                {
                    string localizedFieldName = CurrentLanguageService.GetSufixedLocalizedFieldName(fieldName);
                    if (!readerFields.TryGetValue(localizedFieldName, out fieldIndex) && !pascalReaderFields.TryGetValue(localizedFieldName, out fieldIndex))
                    {
                        throw new InvalidOperationException(string.Format("Field {0} not found in data reader", fieldName));
                    }
                }
                return fieldIndex;
            }

            private Dictionary<string, int> readerFields;
            private Dictionary<string, int> pascalReaderFields;

            private void BuildMappings()
            {
                readerFields = new Dictionary<string, int>(Reader.FieldCount, StringComparer.InvariantCultureIgnoreCase);
                pascalReaderFields = new Dictionary<string, int>(Reader.FieldCount, StringComparer.InvariantCultureIgnoreCase);
                for (int i = 0; i < Reader.FieldCount; i++)
                {
                    string fieldName = Reader.GetName(i);
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        readerFields[fieldName] = i;
                        pascalReaderFields[fieldName.ToPascalNamingConvention()] = i;
                    }
                }
                this.KeyFieldsMappings = new int[this.PivotDef.GroupByFields.Length];
                for (int i = 0; i < this.PivotDef.GroupByFields.Length; i++)
                {
                    this.KeyFieldsMappings[i] = GetFieldIndex(this.PivotDef.GroupByFields[i]);
                }
                this.PivotColumnsMappings = new int[this.PivotDef.PivotColumns.Length];
                this.ValueColumnsMappings = new int[this.PivotDef.PivotColumns.Length];
                for (int i = 0; i < this.PivotDef.PivotColumns.Length; i++)
                {
                    var pivotColumn = this.PivotDef.PivotColumns[i];
                    this.PivotColumnsMappings[i] = GetFieldIndex(pivotColumn.PivotColumnName);
                    this.ValueColumnsMappings[i] = GetFieldIndex(pivotColumn.ValueColumnName);
                }
                readerFields = null;

                pascalReaderFields = null;
            }

            public DataTable Build()
            {
                BuildMappings();

                TargetTable = new DataTable();
                TargetTable.BeginLoadData();
                AddGroupByFieldsColumsToDataTable();

                while (Reader.Read())
                {
                    ReadCurrentKey();
                    AddNewRowIfNeeded();
                    SetColumnValues();
                }
                if (CurrentRow != null) this.TargetTable.Rows.Add(CurrentRow);

                ReorderColumns();

                TargetTable.AcceptChanges();
                TargetTable.EndLoadData();
                return TargetTable;
            }

            private void ReorderColumns()
            {
                this.PivotedColumns.Sort(this.PivotedColumnComparison);
                int ordinalBase = this.PivotDef.GroupByFields.Length;
                for (int i = 0; i < this.PivotedColumns.Count; i++)
                {
                    var pivotedColumn = this.PivotedColumns[i];
                    this.TargetTable.Columns[pivotedColumn.ColumnName].SetOrdinal(ordinalBase + i);
                }
            }

            private void SetColumnValues()
            {
                for (int i = 0; i < this.PivotDef.PivotColumns.Length; i++)
                {
                    var pivotColumn = this.PivotDef.PivotColumns[i];
                    object pivotColumnValue = Reader[this.PivotColumnsMappings[i]];
                    string targetColumnName = pivotColumn.GetTargetColumnName(pivotColumnValue);
                    DataColumn targetColumn = this.TargetTable.Columns[targetColumnName];
                    if (targetColumn == null)
                    {
                        targetColumn = this.TargetTable.Columns.Add(targetColumnName, Reader.GetFieldType(this.ValueColumnsMappings[i]));
                        PivotedColumns.Add(new PivotedColumn(pivotColumn.PivotColumnName, pivotColumnValue, i, targetColumnName));
                    }
                    CurrentRow[targetColumn] = Reader[this.ValueColumnsMappings[i]];
                }
            }

            private void AddNewRowIfNeeded()
            {
                if (!Record.Equals(this.PreviousKey, this.CurrentKey))
                {
                    if (CurrentRow != null) this.TargetTable.Rows.Add(CurrentRow);

                    CurrentRow = this.TargetTable.NewRow();
                    for (int i = 0; i < this.CurrentKey.FieldCount; i++)
                    {
                        CurrentRow[i] = this.CurrentKey[i];
                    }
                    SwapPreviousAndCurrentKeys();
                }
            }

            private void AddGroupByFieldsColumsToDataTable()
            {
                for (int i = 0; i < this.PivotDef.GroupByFields.Length; i++)
                {
                    string keyField = this.PivotDef.GroupByFields[i];
                    TargetTable.Columns.Add(keyField, Reader.GetFieldType(this.KeyFieldsMappings[i]));
                }
            }
        }
    }
}
