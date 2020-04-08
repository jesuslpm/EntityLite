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
using System.Data;
using inercya.EntityLite.Collections;
using System.Collections;

namespace inercya.EntityLite.Extensions
{
	public static class EnumerableExtensions
	{
		public static IDictionary<TKey, IList<TItem>> ToListDictionary<TKey, TItem>(this IEnumerable<TItem> itemsSource, Func<TItem, TKey> keySelector)
		{
            IDictionary<TKey, IList<TItem>> dic;
            if (typeof(TKey) == typeof(string))
            {
                dic =  new Dictionary<TKey, IList<TItem>>((IEqualityComparer<TKey>) StringComparer.CurrentCultureIgnoreCase);
            }
            else
            {
                dic = new Dictionary<TKey, IList<TItem>>();
            }
			
			foreach (var item in itemsSource)
			{
				IList<TItem> list = null;
				TKey key = keySelector(item);
				if (!dic.TryGetValue(key, out list))
				{
					list = new List<TItem>();
					dic.Add(key, list);
				}
				list.Add(item);

			}
			return dic;
		}

		public static HashSet<TKey> ToHashSet<TKey, TItem>(this IEnumerable<TItem> items, Func<TItem, TKey> keySelector)
		{
			HashSet<TKey> hashSet = new HashSet<TKey>();
			foreach (var item in items)
			{
				hashSet.Add(keySelector(item));
			}
			return hashSet;
		}

		public static void FillDetails<TMaster, TDetail, TKey>(this IEnumerable<TMaster> masterItems, Func<TMaster, TKey> keySelector,  Func<TMaster, IList<TDetail>> detailsSelector,  IEnumerable<TDetail> allDetails, Func<TDetail, TKey> foreignKeySelector )
		{
			var dic = masterItems.ToDictionary(keySelector);
			foreach (var detail in allDetails)
			{
				if (dic.TryGetValue(foreignKeySelector(detail), out TMaster masterItem))
				{
					var details = detailsSelector(masterItem);
					if (details == null)
					{
						throw new ArgumentException("All master items must have the details property initialized to a list of detail items, but it is null. Please intilize all details to an empty list before calling this method or call the FillDetails overlod that has detailesInitializer argument");
					}
					details.Add(detail);
				}
			}
		}

		public static void FillDetails<TMaster, TDetail, TKey>(this IEnumerable<TMaster> masterItems, Func<TMaster, TKey> keySelector, Func<TMaster, IList<TDetail>> detailsSelector, Action<TMaster> detailsInitializer, IEnumerable<TDetail> allDetails, Func<TDetail, TKey> foreignKeySelector)
		{
			var dic = masterItems.ToDictionary(keySelector);
			foreach (var detail in allDetails)
			{
				if (dic.TryGetValue(foreignKeySelector(detail), out TMaster masterItem))
				{
					var details = detailsSelector(masterItem);
					if (details == null)
					{
						detailsInitializer(masterItem);
						details = detailsSelector(masterItem);
						if (details == null)
						{
							throw new ArgumentException("Invalid details initializer, it doesn't initialize the details property. The details property is still null after calling the details initializer");
						}
					}
					details.Add(detail);
				}
			}
		}

		public static IList<MasterDetail<M, D>> JoinMasterDetail<M, TKey, D>(IEnumerable<M> masters, Func<M, TKey> GetKey, IEnumerable<D> details, Func<D, TKey> GetMasterKey)
		{
			var masterDetails = new List<MasterDetail<M, D>>();
			var dic = new Dictionary<TKey, MasterDetail<M, D>>();
			foreach (var m in masters)
			{
				var md = new MasterDetail<M, D> { Master = m, Details = new List<D>() };
				var key = GetKey(m);
				if (key != null)
				{
					dic.Add(key, md);
				}
				masterDetails.Add(md);
			}

			foreach (var d in details)
			{
				MasterDetail<M, D> md = null;
				if (dic.TryGetValue(GetMasterKey(d), out md))
				{
					md.Details.Add(d);
				}
			}
			return masterDetails;
		}

		public static IList<MasterDetail<M, D1, D2>> JoinMasterDetail<M, TKey, D1, D2>(this IEnumerable<M> masters, Func<M, TKey> GetKey, IEnumerable<D1> details1, Func<D1, TKey> GetMasterKey1, IEnumerable<D2> details2, Func<D2, TKey> GetMasterKey2)
		{
			var masterDetails = new List<MasterDetail<M, D1, D2>>();
			var dic = new Dictionary<TKey, MasterDetail<M, D1, D2>>();
			foreach (var m in masters)
			{
				var md = new MasterDetail<M, D1, D2> { Master = m, Details1 = new List<D1>(), Details2 = new List<D2>() };
				var key = GetKey(m);
				if (key != null)
				{
					dic.Add(key, md);
				}
				masterDetails.Add(md);
			}

			foreach (var d in details1)
			{
				MasterDetail<M, D1, D2> md = null;
				if (dic.TryGetValue(GetMasterKey1(d), out md))
				{
					md.Details1.Add(d);
				}
			}

			foreach (var d in details2)
			{
				MasterDetail<M, D1, D2> md = null;
				if (dic.TryGetValue(GetMasterKey2(d), out md))
				{
					md.Details2.Add(d);
				}
			}
			return masterDetails;
		}

		public static IList<TreeNode<T>> ToTree<T, TKey>(this IEnumerable<T> entities, Func<T, TKey> keySelector, Func<T, TKey> parentKeySelector, Func<T, bool> mayHaveParent)
		{
			var nodesDic = entities.ToDictionary(keySelector, x => new TreeNode<T> { Entity = x });
			var rootNodes = new List<TreeNode<T>>();

			foreach (var entity in entities)
			{
				var node = nodesDic[keySelector(entity)];
				TreeNode<T> parent = null;
				if (mayHaveParent(entity) && nodesDic.TryGetValue(parentKeySelector(entity), out parent))
				{
					node.Parent = parent;
					parent.Children.Add(node);
				}
				else
				{
					rootNodes.Add(node);
				}
			}
			return rootNodes;
		}


		public static bool HasTheSameItemsAs<T>(this IEnumerable<T> enumerable, IEnumerable<T> other) where T : class
		{
			IEnumerator<T> enumerator1 = enumerable.GetEnumerator();
			IEnumerator<T> enumerator2 = other.GetEnumerator();
			while (true)
			{
				bool next1 = enumerator1.MoveNext();
				bool next2 = enumerator2.MoveNext();
				if (next1 != next2) return false;
				if (!next1) return true;
				if (!enumerator1.Current.IsEqualsTo(enumerator2.Current)) return false;

			}
		}

        public static string Join(this IEnumerable<string> strings, string separator = ", ")
        {
            StringBuilder sb = new StringBuilder();
            bool firstTime = true;
            foreach (var str in strings)
            {
                if (firstTime) firstTime = false;
                else sb.Append(separator);
                sb.Append(str);
            }
            return sb.ToString();
        }

        public static void EnsureField(this List<string> sortedFields, EntityMetadata metadata, string fieldName)
        {
            if (!metadata.Properties.ContainsKey(fieldName)) return;
            int position = sortedFields.BinarySearch(fieldName, StringComparer.InvariantCultureIgnoreCase);
            if (position < 0)
            {
                sortedFields.Insert(~position, fieldName);
            }
        }

        public static void EnsurePrimaryKey(this List<string> sortedFields, EntityMetadata metadata)
        {
            foreach (var primaryKeyPropertyName in metadata.PrimaryKeyPropertyNames)
            {
                sortedFields.EnsureField(metadata, primaryKeyPropertyName);
            }
        }

        public static IDataReader ToDataReader<T>(this IEnumerable<T> collection) where T:class
        {
            return new CollectionDataReader<T>(collection);
        }

        public static bool AreThereMoreThan(this IEnumerable enumerable, int n)
        {
            ICollection collection = enumerable as ICollection;
            if (collection != null) return collection.Count > n;
            var count = 0;
            foreach (var item in enumerable)
            {
                count++;
                if (count > n) return true;
            }
            return count > n;
        }

        public static bool AreThereMoreThan<T>(this IEnumerable<T> enumerable, int n)
        {
            ICollection<T> collection = enumerable as ICollection<T>;
            if (collection != null) return collection.Count > n;
            var count = 0;
            foreach (var item in enumerable)
            {
                count++;
                if (count > n) return true;
            }
            return count > n;
        }
    }
}
