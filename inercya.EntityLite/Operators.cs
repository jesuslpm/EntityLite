using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public enum OperatorLite
    {
        None = 0,
        IsNull,
        IsNotNull,
        Equals,
        NotEquals,
        StartsWith,
        NotStartsWith,
        Contains,
        NotContains,
        Less,
        LessOrEquals,
        Greater,
        GreaterOrEquals,
        In,
        NotIn,
        IsDescendantOf,
        IsChildOf,
        IsGrandChildOf,
        HierarchyLevelEquals,
        STEquals,
        STIntersects,
        STDistanceLess,
        STDistanceLessOrEquals
    }

    public enum LogicalOperatorLite
    {
        And,
        Or
    }
}
