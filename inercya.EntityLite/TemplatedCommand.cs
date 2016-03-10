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

using inercya.EntityLite.Templates;
using inercya.EntityLite.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data;

namespace inercya.EntityLite
{
    public class TemplatedCommand : AbstractCommand
    {
        public readonly ISqlTemplate Template;

        public TemplatedCommand(DataService dataService, ISqlTemplate template) : base(dataService, true)
        {
            if (dataService == null) throw new ArgumentNullException("dataService");
            if (template == null) throw new ArgumentNullException("template");
            this.Template = template;
        }

        protected override  DbCommand GetCommand()
        {
            DbCommand command = this.DataService.EntityLiteProvider.CreateCommand();
            string parameterPrefix = this.DataService.EntityLiteProvider.ParameterPrefix;
            command.CommandText = Template.GetSql(parameterPrefix);
            Template.AddParametersToCommand(command, parameterPrefix);
            return command;
        }


        protected override void SetOutPutParameters(DbCommand command)
        {
            var setters = this.Template.GetType().GetPropertySetters();
            foreach (var p in command.Parameters.Cast<DbParameter>().Where(x => (x.Direction & ParameterDirection.Output) == ParameterDirection.Output))
            {
                if (p.Value == DBNull.Value)
                {
                    setters[p.SourceColumn](this.Template, null);
                }
                else
                {
                    setters[p.SourceColumn](this.Template, p.Value);
                }
            }
        }
    }
}
