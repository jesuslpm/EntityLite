using inercya.EntityLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.EntityLiteIntegration
{
    internal class LockQuery<T> : inercya.EntityLite.Templates.ISqlTemplate
    {
        private readonly DataService ds;
        private readonly EntityMetadata metadata;

        public LockQuery(DataService ds)
        {
            this.ds = ds;
            this.metadata = EntityMetadata.GetEntityMetadata(typeof(T));
            if (this.metadata == null)
            {
                throw new InvalidOperationException($"EntityMetadata not found for type {typeof(T).FullName}");
            }
        }

        public string TransformText()
        {
            var fullTableName = this.metadata.GetFullTableName(ds.EntityLiteProvider.DefaultSchema, ds.EntityLiteProvider.StartQuote, ds.EntityLiteProvider.EndQuote);
            return $"SELECT * FROM {fullTableName} WITH (UPDLOCK, ROWLOCK)";
        }
    }
}
