using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.EntityLiteIntegration
{
    internal class OutboxLockQuery : inercya.EntityLite.Templates.ISqlTemplate
    {
        public string TransformText()
        {
            return "SELECT TOP 1 * FROM mt.outbox_states WITH (UPDLOCK, ROWLOCK, READPAST) ORDER BY created";
        }
    }
}
