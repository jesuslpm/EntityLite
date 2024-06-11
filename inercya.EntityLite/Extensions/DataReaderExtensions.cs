﻿/*
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
using System.Data;
using inercya.EntityLite.Builders;
using inercya.EntityLite.Collections;
using System.Globalization;
#if (NET452 || NETSTANDARD2_0)
using System.Threading.Tasks;
#endif
using System.Data.Common;


#pragma warning disable CA1062 // Validate arguments of public methods. Don't want to do it for performance reasons.
namespace inercya.EntityLite.Extensions
{

    public static class DataReaderExtensions
    {
        public static byte[] GetByteArray(this IDataRecord dataRecord, int fieldOrdinal)
        {

            return (byte[])dataRecord.GetValue(fieldOrdinal);

        }

        public static TimeSpan GetTimeSpan(this IDataRecord dataRecord, int fieldOrdinal)
        {
            return (TimeSpan)dataRecord.GetValue(fieldOrdinal);
        }

        public static UInt64 GetUInt64(this IDataRecord dataRecord, int fieldOrdinal)
        {
            return (UInt64)dataRecord.GetValue(fieldOrdinal);
        }

        public static UInt32 GetUInt32(this IDataRecord dataRecord, int fieldOrdinal)
        {
            return (UInt32)dataRecord.GetValue(fieldOrdinal);
        }

        public static UInt16 GetUInt16(this IDataRecord dataRecord, int fieldOrdinal)
        {
            return (UInt16)dataRecord.GetValue(fieldOrdinal);
        }

        public static DateTimeOffset GetDateTimeOffset(this IDataRecord dataRecord, int fieldOrdinal)
        {
            return (DateTimeOffset)dataRecord.GetValue(fieldOrdinal);
        }

        //public static SqlHierarchyId GetSqlHierarchyId(this IDataRecord dataRecord, int fieldOrdinal)
        //{
        //    return (SqlHierarchyId)dataRecord.GetValue(fieldOrdinal);
        //}

        //public static SqlGeometry GetSqlGeometry(this IDataRecord dataRecord, int fieldOrdinal)
        //{
        //    return (SqlGeometry)dataRecord.GetValue(fieldOrdinal);
        //}

        //public static SqlGeography GetSqlGeography(this IDataRecord dataRecord, int fieldOrdinal)
        //{
        //    return (SqlGeography)dataRecord.GetValue(fieldOrdinal);
        //}

        public static sbyte GetSByte(this IDataRecord dataRecord, int fieldOrdinal)
        {
            return (sbyte)dataRecord.GetValue(fieldOrdinal);
        }

        private static string GetMetadataSummary(this IDataReader reader)
        {
			if (reader == null) throw new ArgumentNullException(nameof(reader));
			var builder = new StringBuilder(1024);
			for (int i = 0; i < reader.FieldCount; i++)
			{
				string fieldName = reader.GetName(i);
				if (fieldName == null) fieldName = string.Empty;
				builder.Append(fieldName).Append('#');
				Type type = reader.GetFieldType(i);
				if (type == null)
				{
					if (string.IsNullOrEmpty(fieldName))
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "data reader field ordinal {0} returned null data type. This might be caused by the lack of some assemblies such as Microsoft.SqlServer.Types.dll", i));
					}
					else
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "data reader field ordinal {0}, name {1} returned null data type. This might be caused by the lack of some assemblies such as Microsoft.SqlServer.Types.dll", i, fieldName));
					}
				}
				builder.Append(type.Name).Append('|');
			}
			return builder.ToString();
        }

		private static KeyLocker<TypeStringKey> keyLocker = new KeyLocker<TypeStringKey>();
        private static readonly SynchronizedDictionary<TypeStringKey, Func<IDataReader, object>> factoryCache = 
			new SynchronizedDictionary<TypeStringKey, Func<IDataReader, object>>();

        public static Func<IDataReader, object> GetFactory(this IDataReader reader, Type entityType)
        {
			if (reader == null) throw new ArgumentNullException(nameof(reader));
			if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            Func<IDataReader, object> factory1 = null;
            var key = new TypeStringKey(entityType, reader.GetMetadataSummary());

            if (factoryCache.TryGetValue(key, out factory1))
            {
				return factory1;
            }

			return keyLocker.ExecuteSynchronized(key, k =>
			{
				Func<IDataReader, object> factory2;
				if (!factoryCache.TryGetValue(k, out factory2))
				{
					var builder = new FromDataReaderFactoryBuilder(entityType, reader);
					factory2 = builder.CreateFactory();
					factoryCache[key] = factory2;
				}
				return factory2;
			});
        }

		public static object GetScalar(this IDataReader reader)
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));
			if (reader.Read())
			{
				object result = reader[0];
				if (result == DBNull.Value) return null;
				return result;
			}
			return null;
		}

        public static List<T> ToList<T>(this IDataReader reader) where T : class
        {
			return reader.ToEnumerable<T>().ToList();
        }

		public static T FirstOrDefault<T>(this IDataReader reader) where T : class
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));
			using (reader)
			{
				if (reader.Read())
				{
					return (T)reader.GetFactory(typeof(T))(reader);
				}
				else
				{
					return null;
				}
			}
		}

		public static IEnumerable<T> ToEnumerable<T>(this IDataReader reader, Action onEnumerationCompleted = null) where T : class
		{
			if (reader == null) throw new ArgumentNullException(nameof(reader));
            try
            {
                using (reader)
                {
                    var factory = reader.GetFactory(typeof(T));
                    while (reader.Read())
                    {
                        yield return (T)factory(reader);
                    }
                }
            }
            finally
            {
                onEnumerationCompleted?.Invoke();
            }
		}

#if (NET452 || NETSTANDARD2_0)
        public static async Task<List<T>> ToListAsync<T>(this DbDataReader reader, Action onEnumerationCompleted = null) where T : class
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var list = new List<T>();
            try
            {
                using (reader)
                {
                    var factory = reader.GetFactory(typeof(T));
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        list.Add((T)factory(reader));
                    }
                }
                return list;
            }
            finally
            {
                onEnumerationCompleted?.Invoke();
            }
        }
#endif


        [Obsolete("Use ToList instead")]
        public static List<T> GetList<T>(this IDataReader reader) where T : class
        {
            return reader.ToList<T>();
        }

    }
}
#pragma warning restore CA1062 // Validate arguments of public methods
