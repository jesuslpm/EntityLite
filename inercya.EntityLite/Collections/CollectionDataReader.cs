using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Collections;
using inercya.EntityLite;

namespace inercya.EntityLite.Collections
{

    public class CollectionDataReader<T> : IDataReader where T:class
    {
        PropertyGetter[] getters;
        PropertyInfo[] properties;
        Type itemType;
        IEnumerator<T> enumerator;
        private T current;
        private object[] buffer;

        public CollectionDataReader(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            itemType = typeof(T);
            properties = itemType.GetProperties();
            getters = GetPropertyGetters();
            enumerator = collection.GetEnumerator();
            buffer = new object[FieldCount];
        }

        static PropertyGetter ToStringGetter(PropertyGetter getter)
        {
            return (value) => getter(value)?.ToString();
        }

        private PropertyGetter[] GetPropertyGetters()
        {
            PropertyGetter[] getters = new PropertyGetter[this.properties.Length];
            IPropertyGetterDictionary propertyGetters = PropertyHelper.GetPropertyGetters(this.itemType);
            int propertyCount = this.properties.Length;
            for (int index = 0; index < propertyCount; index++)
            {
                var property = properties[index];
                getters[index] = propertyGetters[property.Name];
                var propertyTypeName = property.PropertyType.Name;
                if (propertyTypeName == "JToken" || propertyTypeName == "JObject")
                {
                    getters[index] = ToStringGetter(getters[index]);
                }
            }
            return getters;
        }


        #region IDataReader Members

        public void Close()
        {
            this._isClosed = true;
        }

        public int Depth
        {
            get { return 0; }
        }

        public DataTable GetSchemaTable()
        {
            return null;
        }

        private bool _isClosed;
        public bool  IsClosed
        {
            get { return _isClosed; }
        }


        public bool NextResult()
        {
 	        return false;
        }

        public bool Read()
        {
            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                int fieldCount = this.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    buffer[i] = getters[i](current);
                }
                return true;
            }
            else
            {
                current = null;
                return false;
            }
        }

        public int  RecordsAffected
        {
            get { return -1; }
        }

        #endregion

        #region IDisposable Members

        public bool IsDisposed { get; private set; }

        public void  Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;
            IsDisposed = true;
            if (disposing)
            {
                if (enumerator != null) enumerator.Dispose();
            }
        }

        #endregion

        #region IDataRecord Members

        public int  FieldCount
        {
            get { return this.properties.Length; }
        }
            
        public bool GetBoolean(int i)
        {
            return (bool)this.GetValue(i);
        }

        public byte GetByte(int i)
        {
            return (byte)this.GetValue(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
 	        throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return (char)this.GetValue(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
 	        throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
 	        throw new InvalidCastException();
        }

        public string GetDataTypeName(int i)
        {
            return GetFieldType(i).FullName;
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)this.GetValue(i);
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)this.GetValue(i);
        }

        public double  GetDouble(int i)
        {
            return (double)this.GetValue(i);
        }

        public Type GetFieldType(int i)
        {
            Type propertyType = this.properties[i].PropertyType;
            if (propertyType.IsValueType && propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return propertyType.GetGenericArguments()[0];
            }
            else if (propertyType.Name == "JToken" || propertyType.Name == "JObject")
            {
                return typeof(string);
            }
            else 
            { 
                return propertyType;
            }
        }

        public float  GetFloat(int i)
        {
            return (float)this.GetValue(i);
        }

        public Guid  GetGuid(int i)
        {
            return (Guid)this.GetValue(i);
        }

        public short  GetInt16(int i)
        {
            return (short)this.GetValue(i);
        }

        public int  GetInt32(int i)
        {
            return (int)this.GetValue(i);
        }

        public long  GetInt64(int i)
        {
            return (long)this.GetValue(i);
        }

        public string  GetName(int i)
        {
            return this.properties[i].Name;
        }


        private Dictionary<string, int> _propertyOrdinals;
        private Dictionary<string, int> PropertyOrdinals
        {
            get
            {
                if (_propertyOrdinals == null)
                {
                    _propertyOrdinals = new Dictionary<string, int>();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        _propertyOrdinals.Add(properties[i].Name, i);
                    }
                }
                return _propertyOrdinals;
            }
        }


        public int GetOrdinal(string name)
        {
            return PropertyOrdinals[name];
        }

        public string  GetString(int i)
        {
            return (string)this.GetValue(i);
        }

        public object GetValue(int i)
        {
            if (current == null)
            {
                throw new InvalidOperationException("There is no current record");
            }
            object value = buffer[i];
            if (value == null) return DBNull.Value;
            return value;
        }

        public int GetValues(object[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            int n = 0;
            for (int i = 0; i < this.FieldCount && i < values.Length; i++)
            {
                values[i] = GetValue(i);
                n++;
            }
            return n;
        }

        public bool IsDBNull(int i)
        {
            if (current == null)
            {
                throw new InvalidOperationException("There is no current record");
            }
            return buffer[i] == null;
        }

        public object this[string name]
        {
            get { return this.GetValue(this.GetOrdinal(name)); }
        }

        public object this[int i]
        {
	        get { return this.GetValue(i); }
        }
        #endregion
    }
}
