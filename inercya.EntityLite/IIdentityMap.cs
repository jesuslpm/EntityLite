using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
	public interface IIdentityMap
	{
		void Put<T>(string projectionName, T entity) where T : class;
		T Get<T>(string projectionName, object identity) where T : class;
        void Remove(Type entityType, object identity);
	}
}
