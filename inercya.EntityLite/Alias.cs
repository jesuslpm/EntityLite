using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Alias is not a keyword in c# and we need to be backward compatible")]
    public class Alias
    {
        public string Name { get; set; }
        public Type EntityType { get; set; }

        public Alias()
        {
        }

        public Alias(string name, Type entityType)
        {
            this.Name = name;
            this.EntityType = entityType;
        }
    }
}
