using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public class ErrorOcurredEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }
        public bool WillRetry { get; private set; }

        public ErrorOcurredEventArgs(Exception exception, bool willRetry)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            this.Exception = exception;
        }
    }
}
