using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{

    public enum TextTransform
    {
        None = 0,
        ToUnderscoreLowerCaseNamingConvention = 1,
        ToUnderscoreUpperCaseNamingConvention = 2,
        ToPascalNamingConvention = 4,
        ToLower = 8,
        ToUpper = 16
    }
}
