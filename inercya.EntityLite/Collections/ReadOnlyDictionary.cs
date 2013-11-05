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

        /// <summary>Crea una nueva instancia de tipo <see cref="T:ReadOnlyDictionary`2"></see>    
        /// y copia en ella todos los elementos del objeto especificado <see cref="T:IDictionary`2"></see>,
        ///  para ello usa el comparador de igualdad de clave por defecto.</summary>    
        /// <param name="dictionary">El  <see cref="T:IDictionary`2"></see> cuyos elementos son copiados en el     
        /// nuevo <see cref="T:ReadOnlyDictionary`2"></see>.</param>    
        /// <exception cref="T:System.ArgumentException">dictionary contiene una o mas claves duplicadas.    
        /// </exception>    
        /// <exception cref="T:System.ArgumentNullException">dictionary es null. (<b>Nothing</b> en Visual Basic).</exception>    
        public ReadOnlyDictionary(Dictionary<TKey, TValue> dictionary)    
        {        
            if (dictionary == null) throw new ArgumentNullException("dictionary");        
            this._source = dictionary;    
        }

        /// <summary>Crea una nueva instancia de tipo <see cref="T:ReadOnlyDictionary`2"></see>    
        /// y copia en ella todos los elementos del objeto especificado <see cref="T:IDictionary`2"></see>,
        ///  para ello usa el comparador de igualdad de clave especificado <see cref="T:IEqualityComparer`1"></see>.</summary>    
        /// <param name="dictionary">El  <see cref="T:IDictionary`2"></see> cuyos elementos son copiados en el     
        /// nuevo <see cref="T:ReadOnlyDictionary`2"></see>.</param>   
        /// <param name="comparer">La <see cref="T:IEqualityComparer`1"></see> implementación a usar cuando    
        /// se comparan las claves, o null para usar el <see cref="T:EqualityComparer`1"></see> por defecto para     
        /// el tipo de la clave.</param>    
        /// <exception cref="T:System.ArgumentException">dictionary contiene una o mas claves duplicadas.    
        /// </exception>    
        /// <exception cref="T:System.ArgumentNullException">dictionary es null (<b>Nothing</b> en Visual Basic).</exception>    
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary,         
            IEqualityComparer<TKey> comparer)    
        {        
            if (dictionary == null) throw new ArgumentNullException("dictionary");        
            this._source = new Dictionary<TKey, TValue>(dictionary, comparer);    
        }     
         
        /// <summary>Obtiene el número de duplas (clave, valor) contenidos en <see cref="T:ReadOnlyDictionary`2"></see>.    
        /// </summary>    
        /// <returns>El número de duplas (clave, valor) contenidos en <see cref="T:ReadOnlyDictionary`2"></see>.    
        /// </returns>    
        public int Count 
        { 
            get 
            { 
                return this._source.Count; 
            }
        }

        /// <summary>Determina si el <see cref="T:ReadOnlyDictionary`2"></see> 
        /// contiene la clave especificada.    
        /// </summary>    
        /// <returns>true si el <see cref="T:ReadOnlyDictionary`2"></see> 
        /// contiene un elemento con la clave especificada, en caso contrario, false.</returns>    
        /// <param name="key">La clave a buscar en <see cref="T:ReadOnlyDictionary`2"></see>.
        /// </param>    
        /// <exception cref="T:System.ArgumentNullException">La clave es null (<b>Nothing</b> en Visual Basic).</exception>
        public bool ContainsKey(TKey key)    
        {        
            return this._source.ContainsKey(key);    
        }  

        /// <summary>Obtiene una coleccion que contiene las claves de <see cref="T:ReadOnlyDictionary`2"></see>.    
        /// </summary>    
        /// <returns>Un <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection"></see> que contiene     
        /// todas las claves de <see cref="T:System.Collections.Generic.Dictionary`2"></see>.
        /// </returns>    
        public ICollection<TKey> Keys    
        {        
            get { return this._source.Keys; }    
        }
    
        /// <summary>Obtiene el valor asociado a la clave especificada.</summary>    
        /// <param name="key">La clave del valor a obtener.</param>    
        /// <param name="value">Cuando el método acaba, si la clave ha sido encontrada, contiene el 
        /// valor asociado a dicha clave, en caso contrario, contiene el valor por defecto del tipo del
        /// parámetro. Este parámetro se pasa sin inicializar.</param>    
        /// <returns><b>true</b> si el <see cref="T:ReadOnlyDictionary`2"> contiene un elemento con la     
        /// clave especificada; en caso contrario, <b>false</b>.</returns>
        /// <exception cref="T:System.ArgumentNullException">La clave es null (<b>Nothing</b> en Visual Basic).</exception>
        public bool TryGetValue(TKey key, out TValue value)    
        {        
            return this._source.TryGetValue(key, out value);    
        }

        /// <summary>Obtiene una coleccion que contiene los valores de <see cref="T:ReadOnlyDictionary`2">.</summary>    
        /// <returns>Un objeto ICollection<TValue> que contiene     
        /// todlos los valores de <see cref="T:System.Collections.Generic.Dictionary`2"></see>.
        /// </returns>  
        public ICollection<TValue> Values    
        {
            get { return this._source.Values; }    
        }    
        
        /// <summary>Determina si la <see cref="T:ICollection`1"/> contiene la dupla (clave, valor) especificada. </summary>    
        /// <param name="item">El objeto a buscar en <see cref="T:ICollection`1"/>.</param>    
        /// <returns><b>true</b> si el item se encuentra en <b>ICollection</b>; en caso contrario, <b>false</b>.</returns>    
        /// <exception cref="T:System.ArgumentNullException">El item es null (<b>Nothing</b> en Visual Basic).</exception>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)    
        {        
            return ((ICollection<KeyValuePair<TKey, TValue>>)this._source).Contains(item);    
        }  
  
        /// <summary>    
        /// Copia los elementos de la ICollection en un Array, comenzando por un indice del Array especificado.     
        /// </summary>    
        /// <param name="array">El Array unidimensional en el que se van a copiar los elementos de la      
        /// ICollection. El Array debe tener indexación basada en cero.</param>    
        /// <param name="arrayIndex">El índice (basado en cero) del Array por el que se comenzará a copiar.</param>    
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)    
        {        
            ((ICollection<KeyValuePair<TKey, TValue>>)this._source).CopyTo(array, arrayIndex);    
        }    

        /// <summary>Devuelve true.</summary>    
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly    
        {        
            get { return true; }    
        }    
        
        /// <summary>Devuelve un enumerator que itera a través de la colección.</summary>    
        /// <returns>Un IEnumerator que puede ser usado para iterara a través de la colección.</returns>    
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()    
        {        
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)this._source).GetEnumerator();    
        }

        /// <summary>Devuelve un enumerator que itera a través de la colección.</summary>    
        /// <returns>Un IEnumerator que puede ser usado para iterara a través de la colección.</returns>     
        IEnumerator IEnumerable.GetEnumerator()    
        {        
            return ((IEnumerable)this._source).GetEnumerator();    
        }
        
        /// <summary>Determina si el objeto IDictionary contiene un elemento con la clave especificada.</summary>    
        /// <param name="key">La clave a buscar en el objeto IDictionary.</param>    
        /// <returns><b>true</b> si el IDictionary contiene un elemento con la clave; en caso contrario, <b>false</b>.    
        /// </returns>    
        /// <exception cref="T:System.ArgumentNullException">La clave es null (<b>Nothing</b> en Visual Basic).</exception>
        bool IDictionary.Contains(object key)    
        {        
            return ((IDictionary)this._source).Contains(key);    
        }    

        /// <summary>    
        /// Devuelve un <see cref="IDictionaryEnumerator"/> para el <see cref="IDictionary"/>.
        /// </summary>    
        /// <returns>Un IDictionaryEnumerator para el IDictionary.</returns>    
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

        /// <summary>
        /// Obtiene un objeto ICollection que contiene las claves de <see cref="IDictionary"></see>.    
        /// </summary>    
        /// <returns>Un <see cref="ICollection"></see> que contiene     
        /// todas las claves de <see cref="T:System.Collections.Generic.Dictionary`2"></see>.
        /// </returns>   
        ICollection IDictionary.Keys    
        {        
            get { return ((IDictionary)this._source).Keys; }    
        }    
        
        /// <summary>
        /// Obtiene un objeto ICollection que contiene los valores de <see cref="IDictionary"></see>. 
        ///</summary>    
        ICollection IDictionary.Values    
        {        
            get { return ((IDictionary)this._source).Values; }    
        }
 
        /// <summary>    
        /// Para ver la descripción de este método, véase <see cref="ICollection.CopyTo"/>.     
        /// </summary>    
        /// <param name="array">Un Array unidimensional en el que se copiarán los elementos de la     
        /// ICollection. El Array debe tener indexación basada en cero, no en uno. </param>    
        /// <param name="index">El índice (basado en cero) por el cual se empezará a copiar. </param>    
        void ICollection.CopyTo(Array array, int index)    
        {        
            ((ICollection)this._source).CopyTo(array, index);    
        }  
  
        /// <summary>
        /// Para ver la descripción de este método, véase <see cref="ICollection.IsSynchronized"/>.
        /// </summary>    
        bool ICollection.IsSynchronized    
        {        
            get { return ((ICollection)this._source).IsSynchronized; }    
        }    
        
        /// <summary>
        /// Para ver la descripción de este método, véase <see cref="ICollection.SyncRoot"/>.
        /// </summary>    
        object ICollection.SyncRoot    
        {        
            get { return ((ICollection)this._source).SyncRoot; }    
        }      
        
        /// <summary>
        /// Metodo que lanza la excepcion de Solo Lectura.
        /// </summary>
        private static void ThrowNotSupportedException()    
        {        
            throw new NotSupportedException("Este Diccionario es de solo lectura.");
        }
        

        #region Métodos no soportados debido a la restricción de solo lectura

        /// <summary>Este médoto no esta soportado por <see cref="T:ReadOnlyDictionary`2"/></summary>    
        /// <param name="key">El objeto usado como clave del elemento a añadir.</param>    
        /// <param name="value">El objeto usado como valor del elemento a añadir.</param>    
        /// <exception cref="T:System.NotSupportedException">Se intenta añadir una dupla 
        /// (clave, valor) al <see cref="T:ReadOnlyDictionary`2"></see>.</exception>    
        /// <exception cref="T:System.ArgumentNullException">La clave o el valor son null (<b>Nothing</b> en Visual Basic).</exception>
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            ThrowNotSupportedException();
        }

        /// <summary>Este médoto no esta soportado por <see cref="T:ReadOnlyDictionary`2"/></summary>    
        /// <param name="key"></param>    
        /// <returns></returns>    
        /// <exception cref="T:System.ArgumentNullException">La clave es null (<b>Nothing</b> en Visual Basic).</exception>
        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            ThrowNotSupportedException();
            return false;
        }

        /// <summary>Obtiene el valor asociado a la clave especificada. 
        /// La parte de asignacion no está soportada por <see cref="T:ReadOnlyDictionary`2"/></summary>    
        /// <param name="key">La clave del valor a obtener.</param>    
        /// <returns>El valor asociado a la clave especificada. Si la clave especificada no se encuentra, 
        /// se lanza una <see cref="T:System.Collections.Generic.KeyNotFoundException"></see>, y si se 
        /// intenta realizar una asignacion se lanza una <see cref="T:System.NotSupportedException">.</returns>    
        /// <exception cref="T:System.ArgumentNullException">La clave es null (<b>Nothing</b> en Visual Basic).</exception>    
        /// <exception cref="T:System.NotSupportedException">Se intenta establecer un nuevo valor para la clave especificada.</exception>    
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">La clave no existe.</exception>    
        public TValue this[TKey key]
        {
            get { return this._source[key]; }
            set { ThrowNotSupportedException(); }
        }

        /// <summary>Este método no está soportado por <see cref="T:ReadOnlyDictionary`2"/></summary>    
        /// <param name="item">El objeto a añadir a <see cref="T:ICollection`1"/>.</param>    
        /// <exception cref="T:System.NotSupportedException">Se intenta usar este método.</exception>    
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ThrowNotSupportedException();
        }

        /// <summary>Este método no está soportado por <see cref="T:ReadOnlyDictionary`2"/></summary>
        /// <exception cref="T:System.NotSupportedException">Se intenta usar este método.</exception>    
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            ThrowNotSupportedException();
        }

        /// <summary>Este método no está soportado por <see cref="T:ReadOnlyDictionary`2"/></summary>    
        /// <param name="item">El objeto a eliminar de la ICollection.</param>    
        /// <returns>Devuelve false.</returns>    
        /// <exception cref="T:System.NotSupportedException">Se intenta usar este método.</exception>    
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            ThrowNotSupportedException();
            return false;
        }

        /// <summary>Este método no está soportado por <see cref="T:ReadOnlyDictionary`2"/></summary>    
        /// <param name="key">Clave a añadir.</param>    
        /// <param name="value">Valor a añadir.</param>    
        /// <exception cref="T:System.NotSupportedException">Se intenta usar este método.</exception>    
        void IDictionary.Add(object key, object value)
        {
            ThrowNotSupportedException();
        }

        /// <summary>Este método no está soportado por <see cref="T:ReadOnlyDictionary`2"/></summary>
        /// <exception cref="T:System.NotSupportedException">Se intenta usar este método.</exception>    
        void IDictionary.Clear()
        {
            ThrowNotSupportedException();
        }

        /// <summary>Este método no está soportado por <see cref="T:ReadOnlyDictionary`2"/></summary> 
        /// <param name="key">La clave del elemento a eliminar.</param>    
        /// <exception cref="T:System.NotSupportedException">Se intenta usar este método.</exception>    
        void IDictionary.Remove(object key)
        {
            ThrowNotSupportedException();
        }

        /// <summary>Obtiene el valor asociado a la clave especificada. 
        /// La parte de asignacion no está soportada.</summary>    
        /// <param name="key">La clave del valor a obtener.</param>    
        /// <returns>El elemento asociado a la clave especificada. Si la clave especificada no se encuentra, 
        /// se lanza una <see cref="T:System.Collections.Generic.KeyNotFoundException"></see>, y si se 
        /// intenta realizar una asignacion se lanza una <see cref="T:System.NotSupportedException">.</returns> 
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">La clave no existe.</exception>    
        /// <exception cref="NotSupportedException">Se lanza cuando se intenta asignar un valor.</exception>    
        /// <exception cref="ArgumentNullException">La clave es null (<b>Nothing</b> en Visual Basic).</exception>    
        object IDictionary.this[object key]
        {
            get { return ((IDictionary)this._source)[key]; }
            set { ThrowNotSupportedException(); }
        }

        #endregion
    }
    


    /// <summary>
    /// Clase usada para depurar en ReadOnlyDictionary
    /// </summary>
    /// <typeparam name="TKey">La clave</typeparam>
    /// <typeparam name="TValue">El valor</typeparam>
    internal sealed class ReadOnlyDictionaryDebugView<TKey, TValue>
    {   
        private IDictionary<TKey, TValue> dict;    

        /// <summary>
        /// Constructor de copia. Crea una nueva instancia a partir de la que se pasa por parámetro.
        /// </summary>
        /// <param name="dictionary">Objeto que va a ser copiado.</param>
        public ReadOnlyDictionaryDebugView(ReadOnlyDictionary<TKey, TValue> dictionary)    
        {        
            if (dictionary == null) throw new ArgumentNullException("dictionary");        
            this.dict = dictionary;    
        }    
        
        /// <summary>
        /// Método que devuelve los elementos del Diccionario en un Array.
        /// </summary>
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
