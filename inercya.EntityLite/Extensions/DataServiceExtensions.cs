using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Extensions
{
	public static class DataServiceExtensions
	{
		public static IQueryLite<TEntity> CreateQueryLite<TEntity>(this DataService dataAccess, string projectionName) where TEntity : class, new()
		{
			return new QueryLite<TEntity>(projectionName, dataAccess);
		}

		public static IQueryLite<TEntity> CreateQueryLite<TEntity>(this DataService dataAccess, Projection projection) where TEntity : class, new()
		{
			return new QueryLite<TEntity>(projection, dataAccess);
		}


		private static readonly CacheLite<Type, Func<object>> queryLiteFactoriesCache = new CacheLite<Type, Func<object>>(CreateQueryLiteFactory);

		private static Func<object> CreateQueryLiteFactory(Type entityType)
		{
			return typeof(QueryLite<>).MakeGenericType(entityType).CreateDynamicActivator();
		}

		public static IQueryLite CreateQueryLite(this DataService dataAccess, Type entityType, string projectionName)
		{
			IProjectedQueryLite queryLite = (IProjectedQueryLite)queryLiteFactoriesCache.GetItem(entityType)();
			queryLite.ProjectionName = projectionName;
			queryLite.DataService = dataAccess;
			return queryLite;
		}

		public static IQueryLite CreateQueryLite(this DataService dataAccess, Type entityType, Projection projection)
		{
			return dataAccess.CreateQueryLite(entityType, projection.GetProjectionName());
		}
	}
}
