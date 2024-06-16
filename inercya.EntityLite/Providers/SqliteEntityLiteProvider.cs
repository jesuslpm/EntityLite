﻿/*
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
    public class SqliteEntityLiteProvider : EntityLiteProvider
    {
        public const string ProviderName = "System.Data.SQLite";

        public SqliteEntityLiteProvider(DataService dataService): base(dataService)
        {
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
            if (commandText == null) throw new ArgumentNullException(nameof(commandText));
            commandText.Append(";\nSELECT last_insert_rowid() AS AutoIncrementField;");
        }

        public override string GetNextValExpression(string fullSequenceName)
        {
            throw new NotSupportedException("SQLite doesn't support sequences");
        }

        public override void SetProviderTypeToParameter(IDbDataParameter parameter, int providerType)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            parameter.DbType = (DbType)providerType;
        }
    }
}
