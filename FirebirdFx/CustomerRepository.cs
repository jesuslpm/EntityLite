using FirebirdFx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Entities
{
    public partial class CustomerRepository
    {
        public CommandTemplateResult CommandTemplate()
        {
            var template = new FirebirdFx.CommandTemplate
            {
                CustomerNumber = 1001
            };

            var cmd = new inercya.EntityLite.TemplatedCommand(this.DataService, template);
            return cmd.FirstOrDefault<CommandTemplateResult>(); ;
        }
    }
}
