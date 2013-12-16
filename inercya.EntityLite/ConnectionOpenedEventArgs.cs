using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public class ConnectionOpennedEventArgs : EventArgs
    {
        public DbConnection Connection { get; private set; }
        public ConnectionOpennedEventArgs(DbConnection connection)
        {
            this.Connection = connection;
        }

    }
}
