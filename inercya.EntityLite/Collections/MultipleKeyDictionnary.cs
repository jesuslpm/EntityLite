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
using System.Runtime.Serialization;


namespace inercya.EntityLite.Collections
{
    public struct DoubleKey<TKey1, TKey2>
    {
        public TKey1 Key1;
        public TKey2 Key2;
        public DoubleKey(TKey1 key1, TKey2 key2)
        {
            this.Key1 = key1;
            this.Key2 = key2;
        }

        public override bool Equals(object obj)
        {
            try
            {
                DoubleKey<TKey1, TKey2> other = (DoubleKey<TKey1, TKey2>)obj;
                return this.Equals(other);
            }
            catch
            {
                return false;
            }
        }

        public bool Equals(DoubleKey<TKey1, TKey2> other)
        {
            return (object.Equals(Key1, other.Key1) && object.Equals(Key2, other.Key2));
        }


        public override int GetHashCode()
        {
            int hash1 = Key1 == null ? 0 : Key1.GetHashCode();
            int hash2 = Key2 == null ? 0 : Key2.GetHashCode();
            return hash1 ^ hash2;
        }

        public static bool operator ==(DoubleKey<TKey1, TKey2> dk1, DoubleKey<TKey1, TKey2> dk2)
        {
            return dk1.Equals(dk2);
        }

        public static bool operator !=(DoubleKey<TKey1, TKey2> dk1, DoubleKey<TKey1, TKey2> dk2)
        {
            return !dk1.Equals(dk2);
        }
    }

    public interface IDictionary<TKey1, TKey2, TValue> : IDictionary<DoubleKey<TKey1, TKey2>, TValue>
    {
        TValue this[TKey1 key1, TKey2 key2] { get; set; }
        bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value);
        void Add(TKey1 key1, TKey2 key2, TValue value);
        bool ContainsKey(TKey1 key1, TKey2 key2);
    }


    [Serializable]
    public class Dictionary<TKey1, TKey2, TValue> :
        Dictionary<DoubleKey<TKey1, TKey2>, TValue>,
        IDictionary<TKey1, TKey2, TValue>
    {
        public Dictionary()
            : base()
        {
        }

        public Dictionary(int capacity)
            : base(capacity)
        {
        }

        public Dictionary(IEqualityComparer<DoubleKey<TKey1, TKey2>> comparer)
            : base(comparer)
        {
        }

        public Dictionary(IDictionary<DoubleKey<TKey1, TKey2>, TValue> dictionary)
            : base(dictionary)
        {
        }

        public Dictionary(int capacity, IEqualityComparer<DoubleKey<TKey1, TKey2>> comparer)
            : base(capacity, comparer)
        {
        }

        public Dictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Dictionary(IDictionary<DoubleKey<TKey1, TKey2>, TValue> dictionary, IEqualityComparer<DoubleKey<TKey1, TKey2>> comparer)
            : base(dictionary, comparer)
        {
        }

        public TValue this[TKey1 key1, TKey2 key2]
        {
            get
            {
                return this[new DoubleKey<TKey1, TKey2>(key1, key2)];
            }
            set
            {
                this[new DoubleKey<TKey1, TKey2>(key1, key2)] = value;
            }
        }

        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
        {
            return base.TryGetValue(new DoubleKey<TKey1, TKey2>(key1, key2), out value);
        }

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            base.Add(new DoubleKey<TKey1, TKey2>(key1, key2), value);
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            return base.ContainsKey(new DoubleKey<TKey1, TKey2>(key1, key2));
        }
    }

    public struct TripleKey<TKey1, TKey2, TKey3>
    {
        public TKey1 Key1;
        public TKey2 Key2;
        public TKey3 Key3;
        public TripleKey(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            this.Key1 = key1;
            this.Key2 = key2;
            this.Key3 = key3;
        }

        public override bool Equals(object obj)
        {
            try
            {
                TripleKey<TKey1, TKey2, TKey3> other = (TripleKey<TKey1, TKey2, TKey3>)obj;
                return this.Equals(other);
            }
            catch
            {
                return false;
            }
        }

        public bool Equals(TripleKey<TKey1, TKey2, TKey3> other)
        {
            return object.Equals(Key1, other.Key1) && object.Equals(Key2, other.Key2) && object.Equals(Key3, other.Key3);
        }


        public override int GetHashCode()
        {
            int hash1 = Key1 == null ? 0 : Key1.GetHashCode();
            int hash2 = Key2 == null ? 0 : Key2.GetHashCode();
            int hash3 = Key3 == null ? 0 : Key3.GetHashCode();
            return hash1 ^ hash2 ^ hash3;
        }

        public static bool operator ==(TripleKey<TKey1, TKey2, TKey3> tk1, TripleKey<TKey1, TKey2, TKey3> tk2)
        {
            return tk1.Equals(tk2);
        }

        public static bool operator !=(TripleKey<TKey1, TKey2, TKey3> tk1, TripleKey<TKey1, TKey2, TKey3> tk2)
        {
            return !tk1.Equals(tk2);
        }
    }


    public interface IDictionary<TKey1, TKey2, TKey3, TValue> :
        IDictionary<TripleKey<TKey1, TKey2, TKey3>, TValue>
    {
        TValue this[TKey1 key1, TKey2 key2, TKey3 key3] { get; set; }
        bool TryGetValue(TKey1 key1, TKey2 key2, TKey3 key3, out TValue value);
        void Add(TKey1 key1, TKey2 key2, TKey3 key3, TValue value);
        bool ContainsKey(TKey1 key1, TKey2 key2, TKey3 key3);
    }

    [Serializable]
    public class Dictionary<TKey1, TKey2, TKey3, TValue> :
        Dictionary<TripleKey<TKey1, TKey2, TKey3>, TValue>,
        IDictionary<TKey1, TKey2, TKey3, TValue>
    {
        public Dictionary()
            : base()
        {
        }

        public Dictionary(int capacity)
            : base(capacity)
        {
        }

        public Dictionary(IEqualityComparer<TripleKey<TKey1, TKey2, TKey3>> comparer)
            : base(comparer)
        {
        }

        public Dictionary(IDictionary<TripleKey<TKey1, TKey2, TKey3>, TValue> dictionary)
            : base(dictionary)
        {
        }

        public Dictionary(int capacity, IEqualityComparer<TripleKey<TKey1, TKey2, TKey3>> comparer)
            : base(capacity, comparer)
        {
        }

        public Dictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Dictionary(IDictionary<TripleKey<TKey1, TKey2, TKey3>, TValue> dictionary, IEqualityComparer<TripleKey<TKey1, TKey2, TKey3>> comparer)
            : base(dictionary, comparer)
        {
        }

        public TValue this[TKey1 key1, TKey2 key2, TKey3 key3]
        {
            get
            {
                return this[new TripleKey<TKey1, TKey2, TKey3>(key1, key2, key3)];
            }
            set
            {
                this[new TripleKey<TKey1, TKey2, TKey3>(key1, key2, key3)] = value;
            }
        }

        public bool TryGetValue(TKey1 key1, TKey2 key2, TKey3 key3, out TValue value)
        {
            return base.TryGetValue(new TripleKey<TKey1, TKey2, TKey3>(key1, key2, key3), out value);
        }

        public void Add(TKey1 key1, TKey2 key2, TKey3 key3, TValue value)
        {
            base.Add(new TripleKey<TKey1, TKey2, TKey3>(key1, key2, key3), value);
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return base.ContainsKey(new TripleKey<TKey1, TKey2, TKey3>(key1, key2, key3));
        }
    }
}
