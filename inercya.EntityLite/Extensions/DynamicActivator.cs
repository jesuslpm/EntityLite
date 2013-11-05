using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace inercya.EntityLite.Extensions
{
    public static class DynamicActivatorFactory
    {
		static CacheLite<Type, Func<object>> dynamicActivatorsCache = new CacheLite<Type, Func<object>>(CreateDynamicActivator);

        public static Func<object> GetDynamicActivator(this Type type)
        {
			return dynamicActivatorsCache.GetItem(type);
        }

		public static Func<T> GetDynamicActivator<T>() where T: class, new()
		{
			var activator = GetDynamicActivator(typeof(T));
			return delegate { return (T)activator(); };
		}

		private static DynamicMethod CreateDynamicActivatorMethod(this Type type)
		{
			DynamicMethod dm = new DynamicMethod("DynamicActivator<" + type.FullName + ">", typeof(object), null);
			ConstructorInfo cinfo = type.GetConstructor(new Type[] { });
			if (cinfo == null) throw new ArgumentException(type.FullName + " doesn't have public default constructor", "type");
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Newobj, cinfo);
			il.Emit(OpCodes.Ret);
			return dm;
		}

        public static Func<object> CreateDynamicActivator(this Type type)
        {
			DynamicMethod dm = CreateDynamicActivatorMethod(type);
			return (Func<object>)dm.CreateDelegate(typeof(Func<object>));
        }

		public static Func<T> CreateDynamicActivator<T>() where T: class, new()
		{
			DynamicMethod dm = CreateDynamicActivatorMethod(typeof(T));
			return (Func<T>)dm.CreateDelegate(typeof(Func<T>));
		}
    }
}
