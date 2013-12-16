using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using inercya.EntityLite;
using inercya.EntityLite.Extensions;

namespace inercya.EntityLite
{
    public class IdentityMapLite : IIdentityMap
    {

        Dictionary<Type, Dictionary<object, Dictionary<string, object>>> _map;

        private Dictionary<Type, Dictionary<object, Dictionary<string, object>>> Map
        {
            get
            {
                if (_map == null)
                {
                    _map = new Dictionary<Type, Dictionary<object, Dictionary<string, object>>>();
                }
                return _map;
            }
        }


        public T Get<T>(string projectionName, object identity) where T : class
        {
            Dictionary<object, Dictionary<string, object>> dicIdentity = null;
            Dictionary<string, object> dicProjection = null;
            object entity = null;
            if (Map.TryGetValue(typeof(T), out dicIdentity) && dicIdentity.TryGetValue(identity, out dicProjection) && dicProjection.TryGetValue(projectionName, out entity))
            {
                return (T)entity;
            }
            return null;
        }

        public void Put<T>(string projectionName, T entity) where T : class
        {
            Dictionary<object, Dictionary<string, object>> dicIdentity = null;

            if (!Map.TryGetValue(typeof(T), out dicIdentity))
            {
                dicIdentity = new Dictionary<object, Dictionary<string, object>>();
                Map.Add(typeof(T), dicIdentity);
            }
            Dictionary<string, object> dicProjection = null;
            var identity = entity.GetId();
            if (!dicIdentity.TryGetValue(identity, out dicProjection))
            {
                dicProjection = new Dictionary<string, object>();
                dicIdentity.Add(identity, dicProjection);
            }
            dicProjection[projectionName] = entity;
        }

        public void Remove(Type entityType, object identity)
        {
            Dictionary<object, Dictionary<string, object>> dicIdentity = null;
            if (Map.TryGetValue(typeof(Type), out dicIdentity))
            {
                dicIdentity.Remove(identity);
            }
        }
    }
}
