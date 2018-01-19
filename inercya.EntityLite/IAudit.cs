using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public interface IAudit
    {
        void LogChange(object previousEntity, object currentEntity, List<string> sortedFields, EntityMetadata metadata);

#if (NET452 || NETSTANDARD2_0)
        System.Threading.Tasks.Task LogChangeAsync(object previousEntity, object currentEntity, List<string> sortedFields, EntityMetadata metadata);
#endif
    }
}
