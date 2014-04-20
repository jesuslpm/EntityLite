using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Extensions
{
    public static class PivotExtensions
    {
        public static DataTable Pivot(this IDataReader reader, PivotDef pivotDef)
        {
            var builder = new SimplePivotBuilder(pivotDef, reader);
            return builder.Build();
        }


        private class SimplePivotBuilder
        {
            private readonly PivotDef PivotDef;
            private readonly IDataReader Reader;

            private Record PreviousKey;
            private Record CurrentKey;
            private DataTable TargetTable;
            private DataRow CurrentRow;

            private int[] KeyFieldsMappings;
            private int[] PivotColumnsMappings;
            private int[] ValueColumnsMappings;

            public SimplePivotBuilder(PivotDef pivotDef, IDataReader reader)
            {
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

            private void BuildMappings()
            {
                var readerFields = new Dictionary<string, int>(Reader.FieldCount, StringComparer.InvariantCultureIgnoreCase);
                var pascalReaderFields = new Dictionary<string, int>(Reader.FieldCount, StringComparer.InvariantCultureIgnoreCase);
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
                    int fieldIndex = 0;
                    string keyFieldName = this.PivotDef.GroupByFields[i];
                    if (!readerFields.TryGetValue(keyFieldName, out fieldIndex) && !pascalReaderFields.TryGetValue(keyFieldName, out fieldIndex))
                    {
                        throw new InvalidOperationException(string.Format("Field {0} not found in data reader", keyFieldName));
                    }
                    this.KeyFieldsMappings[i] = fieldIndex;
                }
                this.PivotColumnsMappings = new int[this.PivotDef.PivotColumns.Length];
                this.ValueColumnsMappings = new int[this.PivotDef.PivotColumns.Length];
                for (int i = 0; i < this.PivotDef.PivotColumns.Length; i++)
                {
                    int fieldIndex = 0;
                    var pivotColumn = this.PivotDef.PivotColumns[i];
                    if (!readerFields.TryGetValue(pivotColumn.PivotColumnName, out fieldIndex) && !pascalReaderFields.TryGetValue(pivotColumn.PivotColumnName, out fieldIndex))
                    {
                        throw new InvalidOperationException(string.Format("Field {0} not found in data reader", pivotColumn.PivotColumnName));
                    }
                    this.PivotColumnsMappings[i] = fieldIndex;
                    if (!readerFields.TryGetValue(pivotColumn.ValueColumnName, out fieldIndex) && !pascalReaderFields.TryGetValue(pivotColumn.ValueColumnName, out fieldIndex))
                    {
                        throw new InvalidOperationException(string.Format("Field {0} not found in data reader", pivotColumn.ValueColumnName));
                    }
                    this.ValueColumnsMappings[i] = fieldIndex;
                }
            }

            public DataTable Build()
            {
                TargetTable = new DataTable();
                BuildMappings();
                AddGroupByFieldsColumsToDataTable();
                while (Reader.Read())
                {
                    ReadCurrentKey();
                    AddNewRowIfNeeded();
                    SetColumnValues();
                }
                TargetTable.AcceptChanges();
                return TargetTable;
            }

            private void SetColumnValues()
            {
                for (int i = 0; i < this.PivotDef.PivotColumns.Length; i++)
                {
                    var pivotColumn = this.PivotDef.PivotColumns[i];
                    string targetColumnName = pivotColumn.GetTargetColumnName(Reader[this.PivotColumnsMappings[i]]);
                    DataColumn targetColumn = this.TargetTable.Columns[targetColumnName];
                    if (targetColumn == null)
                    {
                        targetColumn = this.TargetTable.Columns.Add(targetColumnName, Reader.GetFieldType(this.ValueColumnsMappings[i]));
                    }
                    CurrentRow[targetColumn] = Reader[this.ValueColumnsMappings[i]];
                }
            }

            private void AddNewRowIfNeeded()
            {
                if (!Record.Equals(this.PreviousKey, this.CurrentKey))
                {
                    CurrentRow = this.TargetTable.NewRow();
                    for (int i = 0; i < this.CurrentKey.FieldCount; i++)
                    {
                        CurrentRow[i] = this.CurrentKey[i];
                    }
                    this.TargetTable.Rows.Add(CurrentRow);
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
