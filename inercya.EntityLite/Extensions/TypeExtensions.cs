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
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace inercya.EntityLite.Extensions
{
	public static class TypeExtensions
	{

		public static bool IsNullableValueType(this Type type)
		{
			return (type.IsGenericType && type.IsValueType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		public static IPropertyGetterDictionary GetPropertyGetters(this Type type)
		{
			return PropertyHelper.GetPropertyGetters(type);
		}

		public static IPropertySetterDictionary GetPropertySetters(this Type type)
		{
			return PropertyHelper.GetPropertySetters(type);
		}

		public static Type UndelyingType(this Type type)
		{
			if (type.IsNullableValueType())
			{
				return type.GetGenericArguments()[0];
			}
			else
			{
				return type;
			}
		}

		public static IEnumerable<SortDescriptor> GetDefaultSort(this Type entityType)
		{
			DefaultOrderAttribute defaultOrder = (DefaultOrderAttribute)entityType.GetCustomAttributes(typeof(DefaultOrderAttribute), true).FirstOrDefault();
			if (defaultOrder == null)
			{
				var primaryKeys = entityType.GetEntityMetadata().PrimaryKeyPropertyNames;
				foreach (var primaryKeyField in primaryKeys)
				{
					yield return new SortDescriptor(primaryKeyField);
				}
			}
			else
			{
				foreach (var sort in defaultOrder.GetSort())
				{
					yield return sort;
				}
			}
		}

		public static object CreateObject(this Type type)
		{
			return DynamicActivatorFactory.GetDynamicActivator(type)();
		}

		public static string GetPrimaryKeyFieldName(this Type entityType)
		{
			if (entityType == null) throw new ArgumentNullException("entityType");
			EntityMetadata entityMetadata = entityType.GetEntityMetadata();
			if (entityMetadata == null)
			{
				throw new InvalidOperationException(string.Format("{0} has no primary key", entityType.Name));
			}
			ICollection<string> primaryKeyFieldNames = entityMetadata.PrimaryKeyPropertyNames;
			if (primaryKeyFieldNames.Count == 0)
			{
				throw new InvalidOperationException(string.Format("{0} has no primary key", entityType.Name));
			}
			if (primaryKeyFieldNames.Count > 1)
			{
				throw new InvalidOperationException(string.Format("The {0} primary key is multiple", entityType.Name));
			}
			return primaryKeyFieldNames.First();
		}

		public static EntityMetadata GetEntityMetadata(this Type entityType)
		{
			return EntityMetadata.GetEntityMetadata(entityType);
		}

        public static object DataContractDeserialize(this Type type, string xml)
        {
            using (var reader = new StringReader(xml))
            {
                return type.DataContractDeserialize(reader);
            }
        }

        public static object DataContractDeserialize(this Type type, TextReader reader)
        {
            using (var xreader = new XmlTextReader(reader))
            {
                return type.DataContractDeserialize(reader);
            }
        }

        public static object DataContractDeserialize(this Type type, XmlReader reader)
        {
            var serializer = new DataContractSerializer(type);
            return serializer.ReadObject(reader);
        }
	}
}
