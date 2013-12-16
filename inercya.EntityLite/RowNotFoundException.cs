using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public class RowNotFoundException : Exception
    {
        public RowNotFoundException(): base()
        {
        }

        public RowNotFoundException(string message) : base(message)
        {
        }

        public RowNotFoundException(string message, Exception innerException): base(message, innerException)
        {
        }
    }
}
