using inercya.EntityLite.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Providers
{
    public class MySqlEntityLiteProvider : EntityLiteProvider
    {
        public const string ProviderName = "MySql.Data.MySqlClient";
        private readonly DataService DataService;

        public MySqlEntityLiteProvider(DataService dataService)
        {
            this.DataService = dataService;
            if (DataService.ProviderName != ProviderName)
            {
                throw new InvalidOperationException(this.GetType().Name + " is for " + ProviderName + ". Not for " + DataService.ProviderName);
            }
        }


        public override string StartQuote
        {
            get { return "`"; }
        }

        public override string EndQuote
        {
            get { return "`"; }
        }


        public override string Concat(params string[] strs)
        {
            return ConcatByFunction("CONCAT", strs);
        }

        protected override void AppendGetAutoincrementField(StringBuilder commandText, EntityMetadata entityMetadata)
        {
            commandText.Append(";\nSELECT LAST_INSERT_ID() AS AutoIncrementField;");
        }

    }
}
