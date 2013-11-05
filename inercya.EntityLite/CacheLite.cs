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
