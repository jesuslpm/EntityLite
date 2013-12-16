using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Templates
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbParameterAttribute : Attribute
    {
        public DbType DbType { get; set; }

        public ParameterDirection Direction { get; set; }

        public int Size { get; set; }

        public byte Precision { get; set; }

        public byte Scale { get; set; }

        public DbParameterAttribute()
        {
            Direction = ParameterDirection.Input;
        }
    }
}
