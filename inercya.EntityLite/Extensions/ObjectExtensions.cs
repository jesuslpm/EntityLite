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
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace inercya.EntityLite.Extensions
{
	public static class ObjectExtensions
	{
		public static object GetPropertyValue(this object instance, string propertyName)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");
			var type = instance.GetType();
			var getters = type.GetPropertyGetters();
			PropertyGetter getter = null;
			if (getters.TryGetValue(propertyName, out getter))
			{
				return getter(instance);
			}
			else
			{
				throw new ArgumentException(string.Format("{0} is not a public property of {1}", propertyName, type.Name));
			}
		}

		public static void SetPropertyValue(this object instance, string propertyName, object propertyValue)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");
			Type type = instance.GetType();
			PropertySetter setter = null;
			if (PropertyHelper.GetPropertySetters(type).TryGetValue(propertyName, out setter))
			{
				setter(instance, propertyValue);
			}
			else
			{
				throw new ArgumentException(string.Format("{0} is not a public property of {1}", propertyName, type.Name));
			}
		}

		public static object TryGetId(this object entity)
		{
			try
			{
				if (entity == null) return null;
				Type entityType = entity.GetType();
				EntityMetadata entityMetadata = entityType.GetEntityMetadata();
				if (entityMetadata == null) return null;
				ICollection<string> primaryKeyFieldNames = entityMetadata.PrimaryKeyPropertyNames;
				if (primaryKeyFieldNames.Count == 0)
				{
					return null;
				}
				if (primaryKeyFieldNames.Count > 1)
				{
					return null;
				}
				string primaryKeyFieldName = primaryKeyFieldNames.First();
				var propertyGetters = PropertyHelper.GetPropertyGetters(entityType);
				return propertyGetters[primaryKeyFieldName](entity);
			}
			catch
			{
				return null;
			}
		}

		public static object GetId(this object entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");
			Type entityType = entity.GetType();
			EntityMetadata entityMetadata = entityType.GetEntityMetadata();
			ICollection<string> primaryKeyFieldNames = entityMetadata.PrimaryKeyPropertyNames;
			if (primaryKeyFieldNames.Count == 0)
			{
				throw new InvalidOperationException(string.Format("cannot get {0} ID, because it has no primary key", entityType.Name));
			}
			if (primaryKeyFieldNames.Count > 1)
			{
				throw new InvalidOperationException(string.Format("cannot get {0} ID,, because its primary key is multiple", entityType.Name));
			}
			string primaryKeyFieldName = primaryKeyFieldNames.First();
			var propertyGetters = PropertyHelper.GetPropertyGetters(entityType);
			return propertyGetters[primaryKeyFieldName](entity);
		}

		public static IDictionary<string, object> GetPrimaryKey(this object entity)
		{
			if (entity == null) return null;
			Type entityType = entity.GetType();
			EntityMetadata entityMetadata = entityType.GetEntityMetadata(); 
			if (entityMetadata == null) return new Dictionary<string, object>(1);
			ICollection<string> primaryKeyFieldNames = entityMetadata.PrimaryKeyPropertyNames;
			if (primaryKeyFieldNames.Count == 0)
			{
				return new Dictionary<string, object>(1);
			}
			var propertyGetters = PropertyHelper.GetPropertyGetters(entityType);
			return primaryKeyFieldNames.ToDictionary(fn => fn, fn => propertyGetters[fn](entity));
		}


		public static string GetPrimaryKeyFieldName(this object entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");
			return entity.GetType().GetPrimaryKeyFieldName();
		}

		public static bool IsEqualsTo<T>(this T obj, T other) where T : class
		{
			if (obj == null && other == null) return true;
			if (obj == null || other == null) return false;
			IPropertyGetterDictionary getters = PropertyHelper.GetPropertyGetters(typeof(T));
			foreach (var kv in getters)
			{
				if (!object.Equals(kv.Value(obj), kv.Value(other))) return false;
			}
			return true;
		}


		public static void AssignPropertiesFrom(this object target, object source)
		{
			if (target == null) throw new ArgumentNullException("target");
			if (source == null) throw new ArgumentNullException("source");

			Type targetType = target.GetType();
			Type sourceType = source.GetType();

			var setters = PropertyHelper.GetPropertySetters(targetType);
			var getters = PropertyHelper.GetPropertyGetters(sourceType);

			foreach (var kvSetter in setters)
			{
				string propertyName = kvSetter.Key;
				PropertySetter setter = kvSetter.Value;
				PropertyGetter getter = null;
				if (getters.TryGetValue(propertyName, out getter))
				{
					setter(target, getter(source));
				}
			}
		}

        public static string ToDataContractXmlString(this object obj)
        {
            var serializer = new DataContractSerializer(obj.GetType());
            using (var swriter = new StringWriter())
            using (var xwriter = new XmlTextWriter(swriter))
            {
                serializer.WriteObject(xwriter, obj);
                return swriter.ToString();
            }
        }
	}
}
