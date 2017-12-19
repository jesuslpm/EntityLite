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
    public class OracleEntityLiteProvider : BaseOracleEntityLiteProvider
    {
        public const string ProviderName = "Oracle.DataAccess.Client";
        public const string ManagedProviderName = "Oracle.ManagedDataAccess.Client";

        public OracleEntityLiteProvider(DataService dataService): base(dataService)
        {
            if (DataService.ProviderName != ProviderName && DataService.ProviderName != ManagedProviderName)
            {
                throw new InvalidOperationException(this.GetType().Name + " is for " + ProviderName +  " and for " + ManagedProviderName + ". Not for " + DataService.ProviderName);
            }
        }

        private static PropertySetter BindByNameSetter;

        public override DbCommand CreateCommand()
        {
            var command = base.CreateCommand();
            if (BindByNameSetter == null)
            {
                Type commandType = command.GetType();
                var pi = commandType.GetProperty("BindByName");
                if (pi == null)
                {
                    throw new InvalidOperationException("BindByName property not found on type " + commandType.FullName);
                }
                else
                {
                    BindByNameSetter = PropertyHelper.GetPropertySetter(pi);
                }
            }
            BindByNameSetter(command, true);
            return command;
        }
    }
}
