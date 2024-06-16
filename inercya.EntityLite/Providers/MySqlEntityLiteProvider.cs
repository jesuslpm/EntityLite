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

using inercya.EntityLite.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Providers
{
    public class MySqlEntityLiteProvider : EntityLiteProvider
    {
        public const string ProviderName = "MySql.Data.MySqlClient";

        public MySqlEntityLiteProvider(DataService dataService) : base(dataService)
        {
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
            if (commandText == null) throw new ArgumentNullException(nameof(commandText));
            commandText.Append(";\nSELECT LAST_INSERT_ID() AS AutoIncrementField;");
        }

        public override string DualTable
        {
            get
            {
                return "DUAL";
            }
        }

        public override string GetNextValExpression(string fullSequenceName)
        {
            throw new NotSupportedException("MySQL doesn't support sequences");
        }

        private static PropertySetter MySqlDbTypeSetter;

        public override void SetProviderTypeToParameter(IDbDataParameter parameter, int providerType)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (MySqlDbTypeSetter == null)
            {
                var parameterType = parameter.GetType();
                var pi = parameterType.GetProperty("MySqlDbType");
                if (pi == null) throw new InvalidOperationException("MySqlDbType property not found on type " + parameterType.FullName);
                MySqlDbTypeSetter = PropertyHelper.GetPropertySetter(pi);
            }
            MySqlDbTypeSetter(parameter, providerType);
        }
    }
}
