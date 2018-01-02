using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public class FieldReference
    {
        public string FieldName { get; set;}

        public Alias Alias { get; set; }

        public FieldReference()
        {
        }

        public FieldReference(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public FieldReference(string fieldName, Alias alias)
        {
            this.FieldName = fieldName;
            this.Alias = alias;
        }
    }
}
