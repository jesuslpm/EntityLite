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
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace inercya.EntityLite.Collections
{

    /// <summary>Proporciona un diccionario genérico de solo lectura.</summary>
    /// <typeparam name="TKey">La clave</typeparam>
    /// <typeparam name="TValue">El valor</typeparam>
    [Serializable][DebuggerDisplay("Count = {Count}")]
    [ComVisible(false)]
    [DebuggerTypeProxy(typeof(ReadOnlyDictionaryDebugView<,>))]
    public class ReadOnlyDictionary<TKey, TValue> : 
        IDictionary<TKey, TValue>,    
        IEnumerable<KeyValuePair<TKey, TValue>>, 
        IDictionary, 
        ICollection,     
        IEnumerable
    {    
        private readonly Dictionary<TKey, TValue> _source;

        public ReadOnlyDictionary(Dictionary<TKey, TValue> dictionary)    
        {        
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));        
            this._source = dictionary;    
        }

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary,         
            IEqualityComparer<TKey> comparer)    
        {        
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));        
            this._source = new Dictionary<TKey, TValue>(dictionary, comparer);    
        }     
         
       public int Count 
        { 
            get 
            { 
                return this._source.Count; 
            }
        }

        public bool ContainsKey(TKey key)    
        {        
            return this._source.ContainsKey(key);    
        }  
 
        public ICollection<TKey> Keys    
        {        
            get { return this._source.Keys; }    
        }
    
        public bool TryGetValue(TKey key, out TValue value)    
        {        
            return this._source.TryGetValue(key, out value);    
        }

        public ICollection<TValue> Values    
        {
            get { return this._source.Values; }    
        }    
        
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)    
        {        
            return ((ICollection<KeyValuePair<TKey, TValue>>)this._source).Contains(item);    
        }  
  
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)    
        {        
            ((ICollection<KeyValuePair<TKey, TValue>>)this._source).CopyTo(array, arrayIndex);    
        }    
  
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly    
        {        
            get { return true; }    
        }    
        
 
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()    
        {        
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)this._source).GetEnumerator();    
        }

   
        IEnumerator IEnumerable.GetEnumerator()    
        {        
            return ((IEnumerable)this._source).GetEnumerator();    
        }
        
        bool IDictionary.Contains(object key)    
        {        
            return ((IDictionary)this._source).Contains(key);    
        }    

   
        IDictionaryEnumerator IDictionary.GetEnumerator()    
        {        
            return ((IDictionary)this._source).GetEnumerator();    
        }

        /// <summary>Devuelve true.</summary>    
        bool IDictionary.IsFixedSize 
        { 
            get { return true; } 
        }    
        
        /// <summary>Devuelve true.</summary>    
        bool IDictionary.IsReadOnly { 
            get { return true; } 
        }


        ICollection IDictionary.Keys    
        {        
            get { return ((IDictionary)this._source).Keys; }    
        }    
        
  
        ICollection IDictionary.Values    
        {        
            get { return ((IDictionary)this._source).Values; }    
        }
 
 
        void ICollection.CopyTo(Array array, int index)    
        {        
            ((ICollection)this._source).CopyTo(array, index);    
        }  
  

        bool ICollection.IsSynchronized    
        {        
            get { return ((ICollection)this._source).IsSynchronized; }    
        }    
        
 
        object ICollection.SyncRoot    
        {        
            get { return ((ICollection)this._source).SyncRoot; }    
        }      
        

        private static void ThrowNotSupportedException()    
        {        
            throw new NotSupportedException("This dictionary is readonly");
        }
        

        #region Métodos no soportados debido a la restricción de solo lectura

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            ThrowNotSupportedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            ThrowNotSupportedException();
            return false;
        }

        public TValue this[TKey key]
        {
            get { return this._source[key]; }
            set { ThrowNotSupportedException(); }
        }

         void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ThrowNotSupportedException();
        }

         void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            ThrowNotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            ThrowNotSupportedException();
            return false;
        }

        void IDictionary.Add(object key, object value)
        {
            ThrowNotSupportedException();
        }
        void IDictionary.Clear()
        {
            ThrowNotSupportedException();
        }

        void IDictionary.Remove(object key)
        {
            ThrowNotSupportedException();
        }

        object IDictionary.this[object key]
        {
            get { return ((IDictionary)this._source)[key]; }
            set { ThrowNotSupportedException(); }
        }

        #endregion
    }
    


    internal sealed class ReadOnlyDictionaryDebugView<TKey, TValue>
    {   
        private IDictionary<TKey, TValue> dict;    

        public ReadOnlyDictionaryDebugView(ReadOnlyDictionary<TKey, TValue> dictionary)    
        {        
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));        
            this.dict = dictionary;    
        }    
        

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]    
        public KeyValuePair<TKey, TValue>[] Items    
        {        
            get        
            {            
                KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[this.dict.Count];
                this.dict.CopyTo(array, 0);
                return array;
            }    
        }
    }
}
