using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Extensions
{
	public static class DictionaryExtensions
	{
		public static string ToListString(this IDictionary<string, object> self)
		{
			if (self == null) throw new ArgumentNullException("self");
			var strings = self.Select(kv => string.Format("{0}: {1}", kv.Key, kv.Value));
			return string.Join(", ", strings.ToArray());
		}
	}
}
