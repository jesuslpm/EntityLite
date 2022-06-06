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

#if NET35 || NET452
		private static Type[] emtpyTypeArray = new Type[] { };
#else
		private static Type[] emtpyTypeArray = Array.Empty<Type>();
#endif

		private static DynamicMethod CreateDynamicActivatorMethod(this Type type)
		{
			DynamicMethod dm = new DynamicMethod("DynamicActivator<" + type.FullName + ">", typeof(object), null);
			ConstructorInfo cinfo = type.GetConstructor(emtpyTypeArray);
			if (cinfo == null) throw new ArgumentException(type.FullName + " doesn't have public default constructor", nameof(type));
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Newobj, cinfo);
			il.Emit(OpCodes.Ret);
			return dm;
		}

        public static Func<object> CreateDynamicActivator(this Type type)
        {
			if (type == null) throw new ArgumentNullException(nameof(type));
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
