using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using inercya.EntityLite.Extensions;

namespace inercya.EntityLite
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SqlFieldAttribute : Attribute
    {
        public DbType DbType { get; set; }
        public int Size { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public bool IsKey { get; set; }
        public bool IsAutoincrement { get; set; }
        public bool IsReadOnly { get; set; }
        public bool AllowNull { get; set; }
        public string BaseColumnName { get; set; }
        public string BaseTableName { get; set; }

        public SqlFieldAttribute(DbType dbType, int size, byte precision, byte scale)
        {
            this.DbType = dbType;
            this.Size = size;
            this.Precision = precision;
            this.Scale = scale;
            
        }
    }

    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    public class SqlEntityAttribute : Attribute
    {
        public string BaseTableName { get; set; }
        public string SchemaName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultOrderAttribute : Attribute
    {
        public string Order { get; set; }
        public IEnumerable<SortDescriptor> GetSort()
        {
            var tokens = this.Order.Split(',').Select(s => s.Trim());
            foreach (var token in tokens)
            {
                SortDescriptor sort = new SortDescriptor(token.Split(' ').FirstOrDefault().Trim());
                if (token.EndsWith(" DESC"))
                {
                    sort.SortOrder = System.Data.SqlClient.SortOrder.Descending;
                }
                yield return sort;
            }
        }

        public DefaultOrderAttribute(string order)
        {
            this.Order = order;
        }
    }
	
	[AttributeUsage(AttributeTargets.Property)]
	public class LocalizedFieldAttribute : Attribute
	{
	}
}

