using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace inercya.EntityLite
{
    public interface IDbProviderFactories
    {
        DbProviderFactory Get(string providerInvariantName);
        void Register(string providerInvariantName, DbProviderFactory dbProviderFactory);
    }

    internal class DefaultDbProviderFactories : IDbProviderFactories
    {
        ConcurrentDictionary<string, DbProviderFactory> factoriesByName = new ConcurrentDictionary<string, DbProviderFactory>();

        public DbProviderFactory Get(string providerInvariantName)
        {
            return factoriesByName[providerInvariantName];
        }

        public void Register(string providerInvariantName, DbProviderFactory dbProviderFactory)
        {
            factoriesByName[providerInvariantName] = dbProviderFactory;
        }
    }

}
