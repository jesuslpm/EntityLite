using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Reflection.Emit;
using System.Web;
using System.Threading;
using inercya.EntityLite.Collections;

namespace inercya.EntityLite
{

    /// <summary>
    /// Definicion de la firma de los métodos Setters a los que los que los objetos
    /// delegados pueden referenciar.
    /// </summary>
    /// <param name="instance">Objeto de tipo anónimo desde el cual se invoca el método dinámico.</param>
    /// <param name="value">Valor que se va a establecer a la propiedad.</param>
    public delegate void PropertySetter(object instance, object value);

    /// <summary>
    /// Definicion de la firma de los métodos Getters a los que los que los objetos
    /// delegados pueden referenciar.
    /// </summary>
    /// <param name="instance">Objeto de tipo anónimo desde el cual se invoca el método dinámico.</param>
    /// <returns>Objeto de tipo anónimo que devuelve el get.</returns>
    public delegate object PropertyGetter(object instance);

    /// <summary>
    /// Interfaz Diccionario para almacenar los Setters dinámicos.
    /// </summary>
    /// <remarks>
    /// Es un diccionario cuya clave es ti po string es el nombre de la propiedad y el valor es el propoerty setter.
    /// </remarks>
    public interface IPropertySetterDictionary
        : IDictionary<string, PropertySetter>
    {
    }

    /// <summary>
    /// Clase de tipo Diccionario para almacenar los Setters dinámicos. 
    /// Nos permite devolver un diccionario de solo lectura para evitar que 
    /// alguien pueda modificarlo externamente.
    /// </summary>
    /// <remarks></remarks>
    internal class PropertySetterDictionary
        : ReadOnlyDictionary<string, PropertySetter>,
        IPropertySetterDictionary
    {
        public PropertySetterDictionary(Dictionary<string, PropertySetter> dictionary) : base(dictionary)
        { 
        }
    }

    /// <summary>
    /// Interfaz Diccionario para almacenar los Getters dinámicos.
    /// </summary>
    /// <remarks></remarks>
    public interface IPropertyGetterDictionary
        : IDictionary<string, PropertyGetter>
    {
    }

    /// <summary>
    /// Clase de tipo Diccionario para almacenar los Getters dinámicos. 
    /// Nos permite devolver un diccionario de solo lectura para evitar que 
    /// alguien pueda modificarlo externamente.
    /// </summary>
    /// <remarks></remarks>
    internal class PropertyGetterDictionary
        : ReadOnlyDictionary<string, PropertyGetter>,
        IPropertyGetterDictionary
    {
        public PropertyGetterDictionary(Dictionary<string, PropertyGetter> dictionary)
            : base(dictionary)
        {
        }
    }


    /// <summary>
    /// Clase para la creacion dinámica de los metodos getters y setters.
    /// </summary>
    /// <remarks>Clase que contiene los métodos necesarios para a partir de un conjuto de propiedades
    /// crear sus metodos getters y setters dinámicamente y devolverlos como en un 
    /// diccionario de solo lectura, en el que la clave será el nombre de la propiedad y 
    /// el valor un puntero al método.</remarks>
    public static class PropertyHelper
    {

		private static CacheLite<Type, IPropertySetterDictionary> settersCache = new CacheLite<Type, IPropertySetterDictionary>(CreatePropertySetters);
		private static CacheLite<Type, IPropertyGetterDictionary> gettersCache = new CacheLite<Type, IPropertyGetterDictionary>(CreatePropertyGetters);

        #region Setters

        /// <summary>
        /// Método que genera los setters de un determinado tipo de objeto (clase) y los
        /// devuelve en un diccionario que contiene todas las propiedades de la clase
        /// junto con los métodos creados dinámicamente.
        /// </summary>
        /// <param name="type">Tipo (Clase) del que se obtendran las propiedades.</param>
        /// <returns>Diccionario de duplas (NombrePropiedad, MétodoSetDinámico).</returns>
        public static IPropertySetterDictionary GetPropertySetters(Type type)
        {
			return settersCache.GetItem(type);
        }


		private static IPropertySetterDictionary CreatePropertySetters(Type type)
		{
            PropertyInfo[] props = type.GetProperties();
            Dictionary<string, PropertySetter> internalSetters = new Dictionary<string, PropertySetter>(props.Length);
            foreach (PropertyInfo pi in props)
            {
                PropertySetter setter = GetPropertySetter(pi);
                if (setter != null)
                {
                    internalSetters.Add(pi.Name, setter);
                }
            }
            return new PropertySetterDictionary(internalSetters);
		}
        
        /// <summary>
        /// Método que obtiene el Setter de una propiedad especificada 'propertyName'
        /// del tipo (Clase).
        /// </summary>
        /// <param name="type">Tipo de datos (Clase) en el que se busacará la propiedad 'propertyName'.</param>
        /// <param name="propertyName">Nombre de la propiedad que nos interesa del tipo.</param>
        /// <returns>Puntero al método Set para la propiedad 'propertyName' del tipo 'type'.</returns>
        public static PropertySetter GetPropertySetter(Type type, string propertyName)
        {
            PropertyInfo pi = type.GetProperty(propertyName);
            if (pi == null)
            {
                return null;
            }
            return GetPropertySetter(pi);
        }


        /// <summary>
        /// Método que obtiene el Setter de de una propiedad cuya informacion se
        /// pasa por parámtro.
        /// </summary>
        /// <param name="pi">Informacion de la propiedad de la cual se quiere obtener
        /// el método Set dinámico.</param>
        /// <returns>Puntero al método Set para la propiedad especificada en ProperyInfo.</returns>
        public static PropertySetter GetPropertySetter(PropertyInfo pi)
        {
            //Si la propiedad es de solo lectura no tendrá metodo Set.
            MethodInfo mi = pi.GetSetMethod();
            if (mi == null) return null;

            DynamicMethod dm = new DynamicMethod(pi.DeclaringType.FullName + ".Set" + pi.Name, null, new Type[] { typeof(object), typeof(object) }, pi.DeclaringType);

            //Se crea el codigo IL (Código ensamblador para .NET)
            ILGenerator il = dm.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, pi.DeclaringType);
            il.Emit(OpCodes.Ldarg_1);

            if (pi.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, pi.PropertyType);
            }
            else
            {
                il.Emit(OpCodes.Castclass, pi.PropertyType);
            }
            il.Emit(OpCodes.Callvirt, mi);
            il.Emit(OpCodes.Ret);

            return (PropertySetter)dm.CreateDelegate(typeof(PropertySetter));
        }

        #endregion


        #region Getters

        /// <summary>
        /// Método que genera los Getters de un determinado tipo de objeto (Clase) y los
        /// devuelve en un diccionario que contiene todas las propiedades de la clase
        /// junto con los métodos creados dinámicamente.
        /// </summary>
        /// <param name="type">Tipo (Clase) del que se obtendran las propiedades.</param>
        /// <returns>Diccionario de duplas (NombrePropiedad, MétodoGetDinámico).</returns>
        public static IPropertyGetterDictionary GetPropertyGetters(Type type)
        {
            return gettersCache.GetItem(type);
        }


        /// <summary>
        /// Método que genera los Getters de un determinado tipo de objeto (clase) y los
        /// devuelve en un diccionario que contiene todas las propiedades de la clase
        /// junto con los métodos creados dinámicamente.
        /// </summary>
        /// <param name="type">Tipo (Clase) del que se obtendran las propiedades.</param>
        /// <param name="cache">Si es true, el Diccionario creado se alamacenará en caché,
        /// en caso contrario, no se almacenará en caché.</param>
        /// <returns>Diccionario de duplas (NombrePropiedad, MétodoGetDinámico).</returns>
        public static IPropertyGetterDictionary CreatePropertyGetters(Type type)
        {          
            PropertyInfo[] props = type.GetProperties();
            Dictionary<string, PropertyGetter> internalGetters = new Dictionary<string, PropertyGetter>(props.Length);
            foreach (PropertyInfo pi in props)
            {
                PropertyGetter getter = GetPropertyGetter(pi);
                if (getter != null)
                {
                    internalGetters.Add(pi.Name, getter);
                }
            }
            return new PropertyGetterDictionary(internalGetters);
        }



        /// <summary>
        /// Método que obtiene el Getter de una propiedad especificada 'propertyName'
        /// del tipo (Clase).
        /// </summary>
        /// <param name="type">Tipo de datos (Clase) en el que se busacará la propiedad 'propertyName'.</param>
        /// <param name="propertyName">Nombre de la propiedad que nos interesa del tipo.</param>
        /// <returns>Puntero al método Get para la propiedad 'propertyName' del tipo 'type'.</returns>
        public static PropertyGetter GetPropertyGetter(Type type, string propertyName)
        {
            PropertyInfo pi = type.GetProperty(propertyName);
            if (pi == null)
            {
                return null;
            }
            return GetPropertyGetter(pi);
        }

        /// <summary>
        /// Método que obtiene el Getter de de una propiedad cuya información se
        /// pasa por parámtro.
        /// </summary>
        /// <param name="pi">Informacion de la propiedad de la cual se quiere obtener
        /// el método Get dinámico.</param>
        /// <returns>Puntero al método Set para la propiedad especificada en ProperyInfo.</returns>
        public static PropertyGetter GetPropertyGetter(PropertyInfo pi)
        {
            //Si la propiedad es de solo escritura no tendrá metodo Get.
            MethodInfo mi = pi.GetGetMethod();
            if (mi == null) return null;

            DynamicMethod dm = new DynamicMethod(pi.DeclaringType.FullName + ".Get" + pi.Name, typeof(object), new Type[] { typeof(object) });

            ILGenerator il = dm.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, pi.DeclaringType);
            il.Emit(OpCodes.Callvirt, mi);

            if (pi.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, pi.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            return (PropertyGetter)dm.CreateDelegate(typeof(PropertyGetter));
        }

        #endregion

        //public static PropertyGetter GetPropertyGetter( PropertyInfo pi )
        //{
        //    MethodInfo genericHelper = typeof( PropertyHelper ).GetMethod( "PropertyGetterHelper",
        //            BindingFlags.Static | BindingFlags.NonPublic );
        //    MethodInfo constructedHelper = genericHelper.MakeGenericMethod( pi.DeclaringType, pi.PropertyType );
        //    return (PropertyGetter )constructedHelper.Invoke( null, new object[] { pi } );;
        //}

        //private static PropertyGetter PropertyGetterHelper<T, TProperty>( PropertyInfo pi ) where T: class 
        //{
        //    MethodInfo mi = pi.GetGetMethod();
        //    if (mi == null) return null;
        //    Func<T, TProperty> func = ( Func<T, TProperty> )Delegate.CreateDelegate( typeof(Func<T, TProperty> ), pi.GetGetMethod() );
        //    PropertyGetter propertyGetter = delegate( object instance ) { return func((T) instance) ; };
        //    return propertyGetter;
        //}

        //public static PropertySetter GetPropertySetter(PropertyInfo pi)
        //{
        //    MethodInfo genericHelper = typeof(PropertyHelper).GetMethod( "PropertySetterHelper",
        //        BindingFlags.Static | BindingFlags.NonPublic);
        //    MethodInfo constructedHelper = genericHelper.MakeGenericMethod( pi.DeclaringType, pi.PropertyType);
        //    return (PropertySetter) constructedHelper.Invoke( null, new object[] { pi } );
        //}

        //private static PropertySetter PropertySetterHelper<T, TProperty>(PropertyInfo pi) where T:class
        //{
        //    MethodInfo mi = pi.GetSetMethod();
        //    if (mi == null) return null;
        //    Action<T, TProperty> action = ( Action<T, TProperty>) Delegate.CreateDelegate( typeof(Action<T, TProperty>), pi.GetSetMethod());
        //    PropertySetter propertySetter = delegate(object instance, object value) { action((T) instance, (TProperty) value); };
        //    return propertySetter;
        //}
    }
}
