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
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Data.SqlTypes;
using inercya.EntityLite.Extensions;
#if (NET452 || NETSTANDARD2_0)
using System.Threading.Tasks;
#endif

namespace inercya.EntityLite
{
	public static class QueryLiteExtensions
	{

		public static bool IsEmpty(this ICollection<ConditionLite> filter)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			if (filter.Count == 0) return true;
			if (filter.Any(sc => sc.Filter == null)) return false;
			if (filter.Any(sc => sc.Filter != null && !sc.Filter.IsEmpty())) return false;
			return true;
		}

		public static ICollection<ConditionLite> Clone(this ICollection<ConditionLite> filter)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			ICollection<ConditionLite> filterClone = new List<ConditionLite>();
			foreach (var condition in filter)
			{
				var conditionClone = new ConditionLite
				{
					FieldName = condition.FieldName,
					FieldValue = condition.FieldValue,
					LogicalOperator = condition.LogicalOperator,
					Operator = condition.Operator,
					SubQuery = condition.SubQuery == null ? null : (IQueryLite)((ICloneable)condition.SubQuery).Clone()
				};

				if (condition.Filter != null)
				{
					conditionClone.Filter = condition.Filter.Clone();
				}

				filterClone.Add(conditionClone);
			}
			return filterClone;
		}

		public static ICollection<ConditionLite> Where(this ICollection<ConditionLite> existingFilter, ICollection<ConditionLite> filter)
		{
			if (existingFilter == null) throw new ArgumentNullException(nameof(existingFilter));
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			existingFilter.Add(new ConditionLite { LogicalOperator = LogicalOperatorLite.And, Operator = OperatorLite.None, Filter = filter });
			return existingFilter;
		}

		 

		public static ICollection<ConditionLite> Where(this ICollection<ConditionLite> filter, string fieldName, OperatorLite oper)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));
			if (oper != OperatorLite.IsNotNull && oper != OperatorLite.IsNull) throw new ArgumentException("El operador sólo puede ser IsNull o IsNotNull", nameof(oper));
			filter.Add(new ConditionLite
			{
				LogicalOperator = LogicalOperatorLite.And,
				FieldName = fieldName,
				Operator = oper
			});
			return filter;
		}


		public static ICollection<ConditionLite> And(this ICollection<ConditionLite> existingFilter, ICollection<ConditionLite> filter)
		{
			if (existingFilter == null) throw new ArgumentNullException(nameof(existingFilter));
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			return existingFilter.Where(filter);
		}



		public static ICollection<ConditionLite> And(this ICollection<ConditionLite> filter, string fieldName, OperatorLite oper)
		{
			return filter.Where(fieldName, oper);
		}


		public static IQueryLite<TEntity> Where<TEntity>(this IQueryLite<TEntity> query, string fieldName, OperatorLite oper)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));
			query.Filter.Where(fieldName, oper);
			return query;
		}


		public static IQueryLite<TEntity> Where<TEntity>(this IQueryLite<TEntity> query, string fieldName, object fieldValue)
		{
			return query.Where(fieldName, OperatorLite.Equals, fieldValue);
		}


		public static IQueryLite<TEntity> Option<TEntity>(this IQueryLite<TEntity> query, string option)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (option == null) throw new ArgumentNullException(nameof(option));
			query.Options.Add(option);
			return query;
		}

		public static ICollection<ConditionLite> Where(this ICollection<ConditionLite> filter, string fieldName, object fieldValue)
		{
			return filter.Where(fieldName, OperatorLite.Equals, fieldValue);
		}

		public static ICollection<ConditionLite> Where(this ICollection<ConditionLite> filter, string fieldName, OperatorLite oper, object fieldValue)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			if (oper == OperatorLite.IsNotNull || oper == OperatorLite.IsNull || oper == OperatorLite.STDistanceLess || oper == OperatorLite.STDistanceLessOrEquals)
				throw new ArgumentException("Binary operators only", nameof(oper));
			var subQuery = fieldValue as IQueryLite;
			filter.Add(new ConditionLite
			{
				LogicalOperator = LogicalOperatorLite.And,
				FieldName = fieldName,
				Operator = oper,
				FieldValue = subQuery == null ? fieldValue : null,
				SubQuery = subQuery
			});
			return filter;
		}

		public static ICollection<ConditionLite> Where(this ICollection<ConditionLite> filter, string fieldName, OperatorLite oper, object fieldValue, object param)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			if (oper != OperatorLite.STDistanceLess && oper != OperatorLite.STDistanceLessOrEquals)
				throw new ArgumentException("Ternary operators only", nameof(oper));
			filter.Add(new ConditionLite
			{
				LogicalOperator = LogicalOperatorLite.And,
				FieldName = fieldName,
				Operator = oper,
				FieldValue = fieldValue,
				Parameter = param
			});
			return filter;
		}

		public static ICollection<ConditionLite> And(this ICollection<ConditionLite> filter, string fieldName, object fieldValue)
		{
			return filter.Where(fieldName, OperatorLite.Equals, fieldValue);
		}

		public static ICollection<ConditionLite> And(this ICollection<ConditionLite> filter, string fieldName, OperatorLite oper, object fieldValue)
		{
			return filter.Where(fieldName, oper, fieldValue);
		}


		public static IQueryLite<TEntity> Where<TEntity>(this IQueryLite<TEntity> query, ICollection<ConditionLite> filter)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			query.Filter.Where(filter);
			return query;
		}


		public static IQueryLite<TEntity> Where<TEntity>(this IQueryLite<TEntity> query, string fieldName, OperatorLite oper, object fieldValue)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			query.Filter.Where(fieldName, oper, fieldValue);
			return query;
		}

        public static DbCommand GetSelectCommandForDebuggingPurposes(this IQueryLite query)
        {
			if (query == null) throw new ArgumentNullException(nameof(query));
			return ((AbstractQueryLite)query).GetSelectCommand();
        }

		public static IQueryLite<TEntity> Where<TEntity>(this IQueryLite<TEntity> query, string fieldName, OperatorLite oper, object fieldValue, object param)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			query.Filter.Where(fieldName, oper, fieldValue, param);
			return query;
		}

		public static IQueryLite<TEntity> And<TEntity>(this IQueryLite<TEntity> query, ICollection<ConditionLite> filter)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			query.Filter.And(filter);
			return query;
		}

		public static IQueryLite<TEntity> And<TEntity>(this IQueryLite<TEntity> query, string fieldName, OperatorLite oper)
		{
			return query.Where(fieldName, oper);
		}

		public static IQueryLite<TEntity> And<TEntity>(this IQueryLite<TEntity> query, string fieldName, OperatorLite oper, object fieldValue)
		{
			return query.Where(fieldName, oper, fieldValue);
		}

		public static IQueryLite<TEntity> And<TEntity>(this IQueryLite<TEntity> query, string fieldName, OperatorLite oper, object fieldValue, object param)
		{
			return query.Where(fieldName, oper, fieldValue, param);
		}

		public static IQueryLite<TEntity> And<TEntity>(this IQueryLite<TEntity> query, string fieldName, object fieldValue)
		{
			return query.Where(fieldName, OperatorLite.Equals, fieldValue);
		}

		public static ICollection<ConditionLite> Or(this ICollection<ConditionLite> existingFilter, ICollection<ConditionLite> filter)
		{
			if (existingFilter == null) throw new ArgumentNullException(nameof(existingFilter));
			existingFilter.Add(new ConditionLite { LogicalOperator = LogicalOperatorLite.Or, Operator = OperatorLite.None, Filter = filter });
			return existingFilter;
		}

		public static ICollection<ConditionLite> Or(this ICollection<ConditionLite> filter, string fieldName, OperatorLite oper)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			if (oper != OperatorLite.IsNotNull && oper != OperatorLite.IsNull) throw new ArgumentException("El operador sólo puede ser IsNull o IsNotNull", nameof(oper));
			filter.Add(new ConditionLite
			{
				LogicalOperator = LogicalOperatorLite.Or,
				FieldName = fieldName,
				Operator = oper
			});
			return filter;
		}


		public static IQueryLite<TEntity> Or<TEntity>(this IQueryLite<TEntity> query, string fieldName, OperatorLite oper)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (oper != OperatorLite.IsNotNull && oper != OperatorLite.IsNull) throw new ArgumentException("El operador sólo puede ser IsNull o IsNotNull", nameof(oper));
			query.Filter.Or(fieldName, oper);
			return query;
		}

		public static ICollection<ConditionLite> Or(this ICollection<ConditionLite> filter, string fieldName, object fieldValue)
		{
			return filter.Or(fieldName, OperatorLite.Equals, fieldValue);
		}


		public static ICollection<ConditionLite> Or(this ICollection<ConditionLite> filter, string fieldName, OperatorLite oper, object fieldValue)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			if (oper == OperatorLite.IsNotNull || oper == OperatorLite.IsNull) throw new ArgumentException("Binary operators only", nameof(oper));
			var subQuery = fieldValue as IQueryLite;
			filter.Add(new ConditionLite()
			{
				LogicalOperator = LogicalOperatorLite.Or,
				FieldName = fieldName,
				Operator = oper,
				FieldValue = subQuery == null ? fieldValue : null,
				SubQuery = subQuery
			});
			return filter;
		}

		public static ICollection<ConditionLite> Or(this ICollection<ConditionLite> filter, string fieldName, OperatorLite oper, object fieldValue, object param)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			if (fieldName == null) throw new ArgumentNullException(nameof(fieldName));
			if (oper != OperatorLite.STDistanceLess && oper == OperatorLite.STDistanceLessOrEquals) throw new ArgumentException("Ternary operators only", nameof(oper));
			filter.Add(new ConditionLite()
			{
				LogicalOperator = LogicalOperatorLite.Or,
				FieldName = fieldName,
				Operator = oper,
				FieldValue = fieldValue,
				Parameter = param
			});
			return filter;
		}


		public static IQueryLite<TEntity> Or<TEntity>(this IQueryLite<TEntity> query, ICollection<ConditionLite> filter)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			query.Filter.Or(filter);
			return query;
		}

		public static IQueryLite<TEntity> Or<TEntity>(this IQueryLite<TEntity> query, string fieldName, OperatorLite oper, object fieldValue)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			query.Filter.Or(fieldName, oper, fieldValue);
			return query;
		}


		public static IQueryLite<TEntity> Or<TEntity>(this IQueryLite<TEntity> query, string fieldName, OperatorLite oper, object fieldValue, object param)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			query.Filter.Or(fieldName, oper, fieldValue, param);
			return query;
		}
		public static IQueryLite<TEntity> Or<TEntity>(this IQueryLite<TEntity> query, string fieldName, object fieldValue)
		{
			return query.Or(fieldName, OperatorLite.Equals, fieldValue);
		}


		public static IQueryLite<TEntity> OrderBy<TEntity>(this IQueryLite<TEntity> query, params string[] sortFieldNames)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (sortFieldNames == null) throw new ArgumentNullException(nameof(sortFieldNames));
			foreach (var sortFieldName in sortFieldNames)
			{
				query.Sort.Add(new SortDescriptor(sortFieldName));
			}
			return query;
		}


		public static IQueryLite<TEntity> OrderByDesc<TEntity>(this IQueryLite<TEntity> query, params string[] sortFieldNames)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (sortFieldNames == null) throw new ArgumentNullException(nameof(sortFieldNames));
			foreach (var sortFieldName in sortFieldNames)
			{
				query.Sort.Add(new SortDescriptor(sortFieldName, SortOrder.Descending));
			}
			return query;
		}

		public static object Get(this IQueryLite query, object ID)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			string primaryKeyFieldName = query.EntityType.GetPrimaryKeyFieldName();
			var condition = new ConditionLite
			{
				FieldName = primaryKeyFieldName,
				Operator = OperatorLite.Equals,
				FieldValue = ID
			};
			query.Filter.Add(condition);
			return query.FirstOrDefault();
		}

		private static object Get<TPrimaryKey>(this IQueryLite query, TPrimaryKey id)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));
			Type entityType = query.EntityType;
			EntityMetadata entityMetadata = entityType.GetEntityMetadata();
			ICollection<string> primaryKeyFieldNames = entityMetadata.PrimaryKeyPropertyNames;
			if (primaryKeyFieldNames.Count == 0)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "cannot get {0} by {1} ID, because it has no primary key", entityType.Name, typeof(TPrimaryKey).Name));
			}
			if (primaryKeyFieldNames.Count > 1)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "cannot get {0} by {1} ID, because its primary key is multiple", entityType.Name, typeof(TPrimaryKey).Name));
			}
			string primaryKeyFieldName = primaryKeyFieldNames.First();
			PropertyInfo pi = entityMetadata.Properties[primaryKeyFieldName].PropertyInfo;
			if (pi == null || pi.PropertyType != typeof(TPrimaryKey))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "cannot get {0} by {1} ID, because its primary key is not an {1}", entityType.Name, typeof(TPrimaryKey).Name));
			}
			query.Filter.Add(new ConditionLite 
			{ 
				FieldName = primaryKeyFieldName, 
				LogicalOperator = LogicalOperatorLite.And, 
				Operator = OperatorLite.Equals,
				FieldValue = id 
			});
			return query.FirstOrDefault();
		}

		public static object Get(this IQueryLite query, int id)
		{
			return query.Get<int>(id);
		}

		private static TEntity Get<TEntity, TPrimaryKey>(IQueryLite<TEntity> query, TPrimaryKey ID) where TEntity : class, new()
		{
			Type entityType = typeof(TEntity);
			EntityMetadata entityMetadata = entityType.GetEntityMetadata(); 
			ICollection<string> primaryKeyFieldNames = entityMetadata.PrimaryKeyPropertyNames;
			if (primaryKeyFieldNames.Count == 0)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "cannot get {0} by {1} ID, because it has no primary key", entityType.Name, typeof(TPrimaryKey).Name));
			}
			if (primaryKeyFieldNames.Count > 1)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "cannot get {0} by {1} ID, because its primary key is multiple", entityType.Name, typeof(TPrimaryKey).Name));
			}
			string primaryKeyFieldName = primaryKeyFieldNames.First();
			PropertyInfo pi = entityMetadata.Properties[primaryKeyFieldName].PropertyInfo;
			if (pi == null || pi.PropertyType != typeof(TPrimaryKey))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "cannot get {0} by {1} ID, because its primary key is not an {1}", entityType.Name, typeof(TPrimaryKey).Name));
			}
			return query.Where(primaryKeyFieldName, OperatorLite.Equals, ID).FirstOrDefault();
		}

		public static TEntity Get<TEntity>(this IQueryLite<TEntity> query, string fieldName, object fieldValue) where TEntity : class, new()
		{
			return query.Where(fieldName, OperatorLite.Equals, fieldValue).FirstOrDefault();
		}

#if (NET452 || NETSTANDARD2_0)
        public static Task<TEntity> GetAsync<TEntity>(this IQueryLite<TEntity> query, string fieldName, object fieldValue) where TEntity : class, new()
        {
            return query.Where(fieldName, OperatorLite.Equals, fieldValue).FirstOrDefaultAsync();
        }
#endif

        /// <summary>
        /// Allows you to specify the fields in the select list
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IQueryLite<TEntity> Fields<TEntity>(this IQueryLite<TEntity> query, FieldsOption options, params string[] fields)
        {
			if (query == null) throw new ArgumentNullException(nameof(query));
            if (fields == null || fields.Length == 0) return query;

            var metadata = typeof(TEntity).GetEntityMetadata();

            var invalidField = fields.FirstOrDefault(x => {
                if (x == "*") return false;
                PropertyMetadata propMetadata;
                if (metadata.Properties.TryGetValue(x, out propMetadata))
                {
                    return propMetadata.SqlField == null  &&  !propMetadata.IsLocalizedFiled;
                }
                return true;
            });

            if (invalidField != null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "\"{0}\" is an invalid field for \"{1}\"", invalidField, typeof(TEntity).Name));
            }

            List<string> sortedFields = new List<string>(fields);

            // Ordenamos los campos por dos razones:
            // 1.- evitar tener que crear un nuero método dinámico de carga de un datareader cuando son los mismos campos especificados en orden diferente
            // 2.- acelerar la búsqueda de los campos de la clave primaria y EntityRowVersion
            sortedFields.Sort(StringComparer.OrdinalIgnoreCase);

            if ((options & FieldsOption.IncludePrimaryKey) != 0)
            {
                sortedFields.EnsurePrimaryKey(metadata);
            }

            if ((options & FieldsOption.IncludeEntityRowVersion) != 0)
            {
                sortedFields.EnsureField(metadata, query.DataService.SpecialFieldNames.EntityRowVersionFieldName);
            }

            query.FieldList = sortedFields;
            return query;
        }

        /// <summary>
        /// Allows you to specify the fields in the select list
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IQueryLite<TEntity> Fields<TEntity>(this IQueryLite<TEntity> query, params string[] fields)
        {
            return query.Fields(FieldsOption.IncludeBoth, fields);
        }

        public static IQueryLite<TEntity> WithAlias<TEntity>(this IQueryLite<TEntity> query, Alias alias)
        {
			if (query == null) throw new ArgumentNullException(nameof(query));
			if (alias == null) throw new ArgumentNullException(nameof(alias));
            if (query.EntityType != alias.EntityType)
            {
                throw new ArgumentException("The entity type of the alias must be the same as the entity type of the query");
            }
            query.Alias = alias;
            return query;
        }

        public static IQueryLite<TEntity> WithAlias<TEntity>(this IQueryLite<TEntity> query, string aliasName)
        {
			if (query == null) throw new ArgumentNullException(nameof(query));
            query.Alias = new Alias(aliasName, typeof(TEntity));
            return query;
        }

        public static IQueryLite<TEntity> WithTimeout<TEntity>(this IQueryLite<TEntity> query, int timeout)
        {
			if (query == null) throw new ArgumentNullException(nameof(query));
			query.CommandTimeout = timeout;
            return query;
        }
	}
}
