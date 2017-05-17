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
using System.Threading;
using System.Data.Common;
using System.Threading.Tasks;

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

        public async static Task<T> ExecuteWithRetriesAsync<T>(this Func<Task<T>> function, int maxRetries, int firstMillisecondsDelay, Action<Exception, bool> onErrorAction = null)
        {
            int retry = 0;
            while (true)
            {
                try
                {
                    return await function().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (retry >= maxRetries)
                    {
                        onErrorAction?.Invoke(ex, false);
                        throw;
                    }
                    onErrorAction?.Invoke(ex, true);
                    await Task.Delay(firstMillisecondsDelay << retry);
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
