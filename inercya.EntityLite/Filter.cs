using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Data.SqlTypes;

namespace inercya.EntityLite
{
	public class FilterLite<TEntity> : List<ConditionLite> where TEntity : class, new()
	{
		public FilterLite() : base()
		{
		}

		public FilterLite(int capacity)
			: base(capacity)
		{
		}

		public FilterLite(IEnumerable<ConditionLite> otherFilter) : base(otherFilter)
		{
		}
	}

}