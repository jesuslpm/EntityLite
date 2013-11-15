using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    [Serializable]
    public class ConditionLite
    {
        public LogicalOperatorLite LogicalOperator { get; set; }
        public string FieldName { get; set; }
        public OperatorLite Operator { get; set; }
        public object FieldValue { get; set; }
        public object Parameter { get; set; }
        public IQueryLite SubQuery { get; set; }
        public ICollection<ConditionLite> Filter { get; set; }
    }
}
