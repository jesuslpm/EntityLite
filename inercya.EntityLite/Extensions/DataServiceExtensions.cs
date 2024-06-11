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

namespace inercya.EntityLite.Extensions
{
	public static class DataServiceExtensions
	{
		public static IQueryLite<TEntity> CreateQueryLite<TEntity>(this DataService dataService, string projectionName) where TEntity : class
		{
			return new QueryLite<TEntity>(projectionName, dataService);
		}

		public static IQueryLite<TEntity> CreateQueryLite<TEntity>(this DataService dataService, Projection projection) where TEntity : class
		{
			return new QueryLite<TEntity>(projection, dataService);
		}


		private static readonly CacheLite<Type, Func<object>> queryLiteFactoriesCache = new CacheLite<Type, Func<object>>(CreateQueryLiteFactory);

		private static Func<object> CreateQueryLiteFactory(Type entityType)
		{
			return typeof(QueryLite<>).MakeGenericType(entityType).CreateDynamicActivator();
		}

		public static IQueryLite CreateQueryLite(this DataService dataService, Type entityType, string projectionName)
		{
			IProjectedQueryLite queryLite = (IProjectedQueryLite)queryLiteFactoriesCache.GetItem(entityType)();
			queryLite.ProjectionName = projectionName;
			queryLite.DataService = dataService;
			return queryLite;
		}

		public static IQueryLite CreateQueryLite(this DataService dataService, Type entityType, Projection projection)
		{
			return dataService.CreateQueryLite(entityType, projection.GetProjectionName());
		}
	}
}
