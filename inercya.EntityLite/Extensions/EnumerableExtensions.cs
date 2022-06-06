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
			if (itemsSource == null) throw new ArgumentNullException(nameof(itemsSource));
			if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
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
			if (items == null) throw new ArgumentNullException(nameof(items));
			if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
			HashSet<TKey> hashSet = new HashSet<TKey>();
			foreach (var item in items)
			{
				hashSet.Add(keySelector(item));
			}
			return hashSet;
		}

		public static void FillDetails<TMaster, TDetail, TKey>(
			this IEnumerable<TMaster> masterItems, 
			Func<TMaster, TKey> keySelector,  
			Func<TMaster, IList<TDetail>> detailsSelector,  
			IEnumerable<TDetail> allDetails, 
			Func<TDetail, TKey> foreignKeySelector )
		{
			if (masterItems == null) throw new ArgumentNullException(nameof(masterItems));
			if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
			if (detailsSelector == null) throw new ArgumentNullException(nameof(detailsSelector));
			if (allDetails == null) throw new ArgumentNullException(nameof(allDetails));
			if (foreignKeySelector == null) throw new ArgumentNullException(nameof(foreignKeySelector));
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

		public static void FillDetails<TMaster, TDetail, TKey>(
			this IEnumerable<TMaster> masterItems,
			Func<TMaster, TKey> keySelector, 
			Func<TMaster, IList<TDetail>> detailsSelector, 
			Action<TMaster> detailsInitializer, 
			IEnumerable<TDetail> allDetails, 
			Func<TDetail, TKey> foreignKeySelector)
		{
			if (masterItems == null) throw new ArgumentNullException(nameof(masterItems));
			if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
			if (detailsSelector == null) throw new ArgumentNullException(nameof(detailsSelector));
			if (allDetails == null) throw new ArgumentNullException(nameof(allDetails));
			if (foreignKeySelector == null) throw new ArgumentNullException(nameof(foreignKeySelector));
			if (detailsInitializer == null) throw new ArgumentNullException(nameof(detailsInitializer));
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

		public static IList<MasterDetail<TMasterItem, TDetailItem>> JoinMasterDetail<TMasterItem, TKey, TDetailItem>(
			IEnumerable<TMasterItem> masterItems, 
			Func<TMasterItem, TKey> getKey, IEnumerable<TDetailItem> detailItems, 
			Func<TDetailItem, TKey> getMasterKey)
		{
			if (masterItems == null) throw new ArgumentNullException(nameof(masterItems));
			if (detailItems == null) throw new ArgumentNullException(nameof(detailItems));
			if (getKey == null) throw new ArgumentNullException(nameof(getKey));
			if (getMasterKey == null) throw new ArgumentNullException(nameof(getMasterKey));
			var masterDetails = new List<MasterDetail<TMasterItem, TDetailItem>>();
			var dic = new Dictionary<TKey, MasterDetail<TMasterItem, TDetailItem>>();
			foreach (var m in masterItems)
			{
				var md = new MasterDetail<TMasterItem, TDetailItem> { Master = m, Details = new List<TDetailItem>() };
				var key = getKey(m);
				if (key != null)
				{
					dic.Add(key, md);
				}
				masterDetails.Add(md);
			}

			foreach (var d in detailItems)
			{
				MasterDetail<TMasterItem, TDetailItem> md = null;
				if (dic.TryGetValue(getMasterKey(d), out md))
				{
					md.Details.Add(d);
				}
			}
			return masterDetails;
		}

		public static IList<MasterDetail<TMasterItem, TDetailItem1, TDetailItem2>> 
			JoinMasterDetail<TMasterItem, TKey, TDetailItem1, TDetailItem2>(
			this IEnumerable<TMasterItem> masterItems, Func<TMasterItem, TKey> keySelector, 
			IEnumerable<TDetailItem1> detailItems1, Func<TDetailItem1, TKey> keySelector1, 
			IEnumerable<TDetailItem2> detailItems2, Func<TDetailItem2, TKey> keySelector2)
		{
			if (masterItems == null) throw new ArgumentNullException(nameof(masterItems));
			if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
			if (keySelector1 == null) throw new ArgumentNullException(nameof(keySelector1));
			if (keySelector2 == null) throw new ArgumentNullException(nameof(keySelector2));
			if (detailItems1 == null) throw new ArgumentNullException(nameof(detailItems1));
			if (detailItems2 == null) throw new ArgumentNullException(nameof(detailItems2));
			var masterDetails = new List<MasterDetail<TMasterItem, TDetailItem1, TDetailItem2>>();
			var dic = new Dictionary<TKey, MasterDetail<TMasterItem, TDetailItem1, TDetailItem2>>();
			foreach (var m in masterItems)
			{
				var md = new MasterDetail<TMasterItem, TDetailItem1, TDetailItem2> { Master = m, Details1 = new List<TDetailItem1>(), Details2 = new List<TDetailItem2>() };
				var key = keySelector(m);
				if (key != null)
				{
					dic.Add(key, md);
				}
				masterDetails.Add(md);
			}

			foreach (var d in detailItems1)
			{
				MasterDetail<TMasterItem, TDetailItem1, TDetailItem2> md = null;
				if (dic.TryGetValue(keySelector1(d), out md))
				{
					md.Details1.Add(d);
				}
			}

			foreach (var d in detailItems2)
			{
				MasterDetail<TMasterItem, TDetailItem1, TDetailItem2> md = null;
				if (dic.TryGetValue(keySelector2(d), out md))
				{
					md.Details2.Add(d);
				}
			}
			return masterDetails;
		}

		public static IList<TreeNode<T>> ToTree<T, TKey>(
			this IEnumerable<T> items, Func<T, TKey> keySelector, 
			Func<T, TKey> parentKeySelector, Func<T, bool> mayHaveParent)
		{
			if (items == null) throw new ArgumentNullException(nameof(items));
			if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
			if (parentKeySelector == null) throw new ArgumentNullException(nameof(parentKeySelector));
			if (mayHaveParent == null) throw new ArgumentNullException(nameof(mayHaveParent));

			var nodesDic = items.ToDictionary(keySelector, x => new TreeNode<T> { Entity = x });
			var rootNodes = new List<TreeNode<T>>();

			foreach (var entity in items)
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


		public static bool HasTheSameItemsAs<T>(this IEnumerable<T> items, IEnumerable<T> otherItems) where T : class
		{
			if (items == null) throw new ArgumentNullException(nameof(items));
			if (otherItems == null) throw new ArgumentNullException(nameof(otherItems));

			using (IEnumerator<T> enumerator1 = items.GetEnumerator())
			using (IEnumerator<T> enumerator2 = otherItems.GetEnumerator())
			{
				while (true)
				{
					bool next1 = enumerator1.MoveNext();
					bool next2 = enumerator2.MoveNext();
					if (next1 != next2) return false;
					if (!next1) return true;
					if (!enumerator1.Current.IsEqualsTo(enumerator2.Current)) return false;
				}
			}
		}

        public static string Join(this IEnumerable<string> strings, string separator = ", ")
        {
			if (strings == null) throw new ArgumentNullException(nameof(strings));
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
			if (sortedFields == null) throw new ArgumentNullException(nameof(sortedFields));
			if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            if (!metadata.Properties.ContainsKey(fieldName)) return;
            int position = sortedFields.BinarySearch(fieldName, StringComparer.InvariantCultureIgnoreCase);
            if (position < 0)
            {
                sortedFields.Insert(~position, fieldName);
            }
        }

        public static void EnsurePrimaryKey(this List<string> sortedFields, EntityMetadata metadata)
        {
			if (sortedFields == null) throw new ArgumentNullException(nameof(sortedFields));
			if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            foreach (var primaryKeyPropertyName in metadata.PrimaryKeyPropertyNames)
            {
                sortedFields.EnsureField(metadata, primaryKeyPropertyName);
            }
        }

        public static IDataReader ToDataReader<T>(this IEnumerable<T> items) where T:class
        {
			if (items == null) throw new ArgumentNullException(nameof(items));
            return new CollectionDataReader<T>(items);
        }

        public static bool AreThereMoreThan(this IEnumerable items, int n)
        {
			if (items == null) throw new ArgumentNullException(nameof(items));
            ICollection collection = items as ICollection;
            if (collection != null) return collection.Count > n;
            var count = 0;
            foreach (var item in items)
            {
                count++;
                if (count > n) return true;
            }
            return count > n;
        }

        public static bool AreThereMoreThan<T>(this IEnumerable<T> items, int n)
        {
			if (items == null) throw new ArgumentNullException(nameof(items));
            ICollection<T> collection = items as ICollection<T>;
            if (collection != null) return collection.Count > n;
            var count = 0;
            foreach (var item in items)
            {
                count++;
                if (count > n) return true;
            }
            return count > n;
        }
    }
}
