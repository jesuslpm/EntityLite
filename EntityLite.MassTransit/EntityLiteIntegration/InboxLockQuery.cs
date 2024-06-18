using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.EntityLiteIntegration
{
    internal class InboxLockQuery : inercya.EntityLite.Templates.ISqlTemplate
    {
        public string TransformText()
        {
            return "SELECT * FROM mt.inbox_states WITH (UPDLOCK, ROWLOCK)";
        }
    }
}
