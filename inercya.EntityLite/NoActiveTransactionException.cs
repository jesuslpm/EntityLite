using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace inercya.EntityLite
{
	[Serializable]
	public class NoActiveTransactionException : Exception
	{
		public NoActiveTransactionException() { }

		public NoActiveTransactionException(string message)
			: base(message) { }

		public NoActiveTransactionException(string message, Exception inner)
			: base(message, inner) { }
	}
}
