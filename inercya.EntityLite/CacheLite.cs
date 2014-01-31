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

using inercya.EntityLite.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
	public class CacheLite<TKey, TValue>
	{
		private KeyLocker<TKey> keyLocker = new KeyLocker<TKey>();
		private SynchronizedDictionary<TKey, TValue> cache = new SynchronizedDictionary<TKey, TValue>();

		public Func<TKey, TValue> Producer { get; set; }

		public CacheLite()
		{
		}

		public CacheLite(Func<TKey, TValue> producer)
		{
			this.Producer = producer;
		}

		public void Remove(TKey key)
		{
			cache.Remove(key);
		}

		public TValue GetItem(TKey key)
		{
			TValue cacheItem;
			if (cache.TryGetValue(key, out cacheItem))
			{
				return cacheItem;
			}

			return keyLocker.ExecuteSynchronized(key, k =>
			{
				TValue item;
				if (!cache.TryGetValue(k, out item))
				{
					item = Producer(k);
					cache.Add(k, item);
				}
				return item;
			});
		}
	}
}
