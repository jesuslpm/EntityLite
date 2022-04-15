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

namespace inercya.EntityLite
{
    public class KeyLocker<TKey>
    {
        private class KeyLock
        {
            public volatile int Count;
        }

        private readonly Dictionary<TKey, KeyLock> keyLocks = new Dictionary<TKey, KeyLock>();

        public T ExecuteSynchronized<T>(TKey key, Func<TKey, T> function)
        {
            KeyLock keyLock = null;
            try
            {
                lock (keyLocks)
                {
                    try
                    { }
                    finally
                    {
                        if (!keyLocks.TryGetValue(key, out keyLock))
                        {
                            keyLock = new KeyLock();
                            keyLocks.Add(key, keyLock);
                        }
                        keyLock.Count++;
                    }
                }
                lock (keyLock)
                {
                    return function(key);
                }
            }
            finally
            {
                lock (keyLocks)
                {
                    if (keyLock != null && --keyLock.Count == 0) keyLocks.Remove(key);
                }
            }
        }

        public void ExecuteSynchronized(TKey key, Action<TKey> action)
        {
            this.ExecuteSynchronized<bool>(key, k =>
            {
                action(k);
                return true;
            });
        }
    }
}
