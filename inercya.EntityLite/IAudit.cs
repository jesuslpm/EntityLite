using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inercya.EntityLite
{
    public interface IAudit
    {
        void LogChange(object previousEntity, object currentEntity, List<string> sortedFields, EntityMetadata metadata);

        Task LogChangeAsync(object previousEntity, object currentEntity, List<string> sortedFields, EntityMetadata metadata);
    }
}
