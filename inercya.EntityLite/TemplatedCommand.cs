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
        public readonly DataService DataService;
        public readonly ISqlTemplate Template;

        public TemplatedCommand(DataService dataService, ISqlTemplate template) : base(dataService, true)
        {
           
            if (template == null) throw new ArgumentNullException("template");
            this.Template = template;
        }

        protected override  DbCommand GetCommand()
        {
            DbCommand command = this.DataService.Connection.CreateCommand();
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
