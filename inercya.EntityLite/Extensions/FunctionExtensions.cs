using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.Common;

namespace inercya.EntityLite.Extensions
{
	public static class FunctionExtensions
	{

		public static T ExecuteWithRetries<T>(this Func<T> function, int maxRetries, int firstMillisecondsDelay, Action<Exception, bool> onErrorAction = null)
		{
			int retry = 0;
			while (true)
			{
				try
				{
					return function();
				}
				catch (Exception ex)
				{

					if (retry >= maxRetries)
					{
						if (onErrorAction != null) onErrorAction(ex, false);
						throw;
					}
					if (onErrorAction != null) onErrorAction(ex, true);
					Thread.Sleep(firstMillisecondsDelay << retry);
					retry++;
				}
			}
		}

		public static IEnumerable<T> ToEnumerable<T>(this Func<DbCommand> createCommand)
		{
			using (var cmd = createCommand())
			using (var reader = cmd.ExecuteReader())
			{
				var factory = reader.GetFactory(typeof(T));
				while (reader.Read())
				{
					yield return (T)factory(reader);
				}
			}
		}
	}
}
