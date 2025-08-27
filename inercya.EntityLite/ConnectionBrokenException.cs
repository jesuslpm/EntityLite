using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace inercya.EntityLite
{
	[Serializable]
	public class ConnectionBrokenException : Exception
	{
		public ConnectionBrokenException() { }

		public ConnectionBrokenException(string message)
			: base(message) { }

		public ConnectionBrokenException(string message, Exception inner)
			: base(message, inner) { }
	}
}
