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
using System.Diagnostics;
using System.Runtime.Serialization;

namespace inercya.EntityLite.Collections
{
    public class SynchronizedDictionary<TKey, TValue>: IDictionary<TKey, TValue>
    {
        private volatile bool isWriterInProgress;
        private volatile int version;

        private Dictionary<TKey, TValue> dictionary;

        public SynchronizedDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
        }

        public SynchronizedDictionary(int capacity)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public SynchronizedDictionary(IEqualityComparer<TKey> comparer)
        {
             dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public SynchronizedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = new Dictionary<TKey,TValue>(dictionary);
        }

        public SynchronizedDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey,TValue>(capacity, comparer);
        }


        public SynchronizedDictionary(IDictionary<TKey,TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        private void SynchronizedWriteAction(Action action)
        {
			SynchronizedWriteFunction<bool>(delegate { action(); return true; });
        }

        private TResult SynchronizedWriteFunction<TResult>(Func<TResult> func)
        {
            lock (dictionary)
            {
                TResult result;
                try {}
				/* 
				 * putting the code in the finally block
				 * prevents a thread abort from corrupting the dictionary.
				 * because a thread abort cannot be raised in a finally block.
				 * We use SynchronizedDictionary to hold shared state such as 
				 * caches, so corruption is unaceptable.
				*/
				finally
                {
                    try
                    {
                        isWriterInProgress = true;
                        result = func();
                    }
                    finally
                    {
                        version++;
                        isWriterInProgress = false;
                    }
                }
                return result;
            }
        }

        private TResult SyncrhonizedReadFunction<TResult>(Func<TResult> func)
        {
            int version;
            int loops = 0;
            //int totalLoops = 0;
            int attempts = 1;
            //int exceptions = 0;
            while (true)
            {
                loops++;
                //totalLoops++;

                version = this.version;
                bool isWriterInProgress = false;
                TResult result = default(TResult);
                try
                {
                    result = func();
                    isWriterInProgress = this.isWriterInProgress;
                }
                catch
                {
                    //exceptions++;
                    isWriterInProgress = true;
                }
                if (attempts > 5)
                {
                    lock (dictionary)
                    {
                        //Console.WriteLine($"Locked read, loops: {totalLoops}, exceptions: {exceptions}");
                        return func();
                    }
                }
                if (isWriterInProgress)
                {
                    if (Environment.ProcessorCount == 1 || (loops & 0x3F) == 0)
                    {
                        attempts++;
                        Thread.Sleep(1);
                    }
                    else if ((loops & 0xF) == 0)
                    {
                        attempts++;
                        Thread.Sleep(0);
                    }
                    else
                    {
                        Thread.SpinWait(16);
                    }
                }
                else if (version != this.version)
                {
                    loops = 0;
                    attempts++;
                }
                else
                {
                    return result;
                }
            }
        }


        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            SynchronizedWriteAction(delegate { dictionary.Add(key, value); });
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return SyncrhonizedReadFunction(delegate { return dictionary.ContainsKey(key); });
        }

        public ICollection<TKey> Keys
        {
            get 
            {
                lock (dictionary)
                {
                    return dictionary.Keys;
                }
            }
        }

        public bool Remove(TKey key)
        {
            return SynchronizedWriteFunction(delegate { return dictionary.Remove(key); });
        }


        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
			TValue resultValue = default(TValue);
			bool result = this.SyncrhonizedReadFunction(delegate
			{
				return  this.dictionary.TryGetValue(key, out resultValue);
			});
			value = resultValue;
			return result;
        }       


        public ICollection<TValue> Values
        {
            get 
            {
                lock (dictionary)
                {
                    return dictionary.Values;
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (this.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            set
            {
                SynchronizedWriteAction(delegate { dictionary[key] = value; });
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey,TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            SynchronizedWriteAction(delegate { dictionary.Clear(); });
        }

        bool ICollection<KeyValuePair<TKey,TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null) throw new ArgumentException("item.key is null");
            return SyncrhonizedReadFunction(delegate { return dictionary.Contains(item); });
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (dictionary)
            {
                ((IDictionary<TKey, TValue>) dictionary).CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get 
            {
                return dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get 
            { 
                return ((IDictionary<TKey, TValue>) dictionary).IsReadOnly; 
            }
        }

        bool ICollection<KeyValuePair<TKey,TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return SynchronizedWriteFunction(delegate { return dictionary.Remove(item.Key); });
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotSupportedException("Enumeration is not thread safe");
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException("Enumeration is not thread safe"); ;
        }
        #endregion
    }
}
