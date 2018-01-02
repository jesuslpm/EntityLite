using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
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
