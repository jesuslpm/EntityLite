/*
Copyright 2014 i-nercya intelligent software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
        public string ColumnName { get; set; }
        public string SequenceName { get; set; }
        public int ProviderType { get; set; }
        public bool IsEncrypted { get; set; }

        public SqlFieldAttribute(DbType dbType, int size)
        {
            this.DbType = dbType;
            this.Size = size;
            this.ProviderType = int.MaxValue;
        }
    }

    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    public class SqlEntityAttribute : Attribute
    {
        public string BaseTableName { get; set; }
        public string SchemaName { get; set; }
        public bool IsAuditable { get; set; }
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

