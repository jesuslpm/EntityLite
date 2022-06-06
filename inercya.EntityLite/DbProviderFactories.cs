using inercya.EntityLite.Collections;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace inercya.EntityLite
{
    public interface IDbProviderFactories
    {
#pragma warning disable CA1716 // need to be backward compatible
        DbProviderFactory Get(string providerInvariantName);
#pragma warning restore CA1716 // Identifiers should not match keywords
        void Register(string providerInvariantName, DbProviderFactory dbProviderFactory);
    }

    internal class DefaultDbProviderFactories : IDbProviderFactories
    {
        SynchronizedDictionary<string, DbProviderFactory> factoriesByName = new SynchronizedDictionary<string, DbProviderFactory>();

        public DbProviderFactory Get(string providerInvariantName)
        {
            Exception ex = null;
            DbProviderFactory dbProviderFactory;
            if (factoriesByName.TryGetValue(providerInvariantName, out dbProviderFactory))
            {
                return dbProviderFactory;
            }

#if (NETSTANDARD2_0 == false)
            try
            {
                dbProviderFactory = System.Data.Common.DbProviderFactories.GetFactory(providerInvariantName);
                factoriesByName[providerInvariantName] = dbProviderFactory;
            }
            catch (Exception x)
            {
                ex = x;
            }
#endif
            if (dbProviderFactory == null)
            {

                string errorMessage = "Unable to find the requested .Net Framework Data Provider: " + providerInvariantName;
#if NETSTANDARD2_0
                errorMessage += ". Did you forget to call ConfigurationLite.DbProviderFactories.Register?";
#else
                errorMessage += ". To make the it available you can call ConfigurationLite.DbProviderFactories.Register or you can include it in DbProviderFactories section of the configuration file.";
#endif
#pragma warning disable CA1508 // It can be not null in NET35 and NET452
                if (ex == null)
#pragma warning restore CA1508 // Avoid dead conditional code
                {
                    throw new ArgumentException(errorMessage, nameof(providerInvariantName));
                }
                else
                {
                    throw new ArgumentException(errorMessage, nameof(providerInvariantName), ex);
                }
            }
            return dbProviderFactory;
        }

        public void Register(string providerInvariantName, DbProviderFactory dbProviderFactory)
        {
            factoriesByName[providerInvariantName] = dbProviderFactory;
        }
    }
}
