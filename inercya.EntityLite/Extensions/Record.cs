using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Extensions
{
    public class Record 
    {

        private object[] fieldValues;

        public Record(int fieldCount)
        {
            this.fieldValues = new object[fieldCount];
        }

        public object this[int fieldIndex]
        {
            get
            {
                return fieldValues[fieldIndex];
            }
            set
            {
                fieldValues[fieldIndex] = value;
            }
        }

        public static bool Equals(Record r1, Record r2)
        {
            if (object.ReferenceEquals(r1, r2)) return true;
            if ((object)r1 == null) return false;
            if ((object)r2 == null) return false;
            if (r1.FieldCount != r2.FieldCount) return false;
            for (int i = 0; i < r1.FieldCount; i++)
            {
                if (!object.Equals(r1[i], r2[i])) return false;
            }
            return true;
        }

        public int FieldCount
        {
            get { return fieldValues.Length; }
        }

        public override int GetHashCode()
        {
            int hash = 23;
            unchecked
            {
                for (int i = 0; i < this.fieldValues.Length; i++)
                {
                    object v = this.fieldValues[i];
                    if (v != null)
                    {
                        hash = hash * 31 + v.GetHashCode();
                    }
                }
                return hash;
            }
        }

        public bool Equals(Record other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Record;
            return Equals(other);
        }

        public static bool operator ==(Record r1, Record r2)
        {
            return Equals(r1, r2);
        }

        public static bool operator !=(Record r1, Record r2)
        {
            return !Equals(r1, r2);
        }
    }
}
