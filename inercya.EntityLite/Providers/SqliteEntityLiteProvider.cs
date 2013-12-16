using inercya.EntityLite.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Providers
{
    public class SqliteEntityLiteProvider : EntityLiteProvider
    {
        public const string ProviderName = "System.Data.SQLite";
        private readonly DataService DataService;

        public SqliteEntityLiteProvider(DataService dataService)
        {
            this.DataService = dataService;
            if (DataService.ProviderName != ProviderName)
            {
                throw new InvalidOperationException(this.GetType().Name + " is for " + ProviderName + ". Not for " + DataService.ProviderName);
            }
        }


        public override string StartQuote
        {
            get { return "["; }
        }

        public override string EndQuote
        {
            get { return "]"; }
        }

        protected override void AppendGetAutoincrementField(StringBuilder commandText, EntityMetadata entityMetadata)
        {
            commandText.Append(";\nSELECT last_insert_rowid() AS AutoIncrementField;");
        }

    }
}
