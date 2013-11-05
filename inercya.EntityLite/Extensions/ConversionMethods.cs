using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace inercya.EntityLite.Extensions
{
    public static class ConversionMethods
    {
        private class ConverterEntry
        {
            public Type FromType;
            public Type ToType;
            public MethodInfo Method;
        }

        private static readonly ConverterEntry[] SpecialConvertersTable = new ConverterEntry[] {
            new ConverterEntry { 
                FromType = typeof(DateTimeOffset), 
                ToType = typeof(DateTime), 
                Method = typeof(ConversionMethods).GetMethod("FromDateTimeOffsetToLocalDateTime") 
            }
        };

        public static DateTime FromDateTimeOffsetToLocalDateTime(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.LocalDateTime;
        }

        public static MethodInfo GetConversionMethod(Type fromType, Type toType)
        {
            MethodInfo converter = GetConversionOperatorMethodInfo(fromType, toType);
            if (converter != null) return converter;
            return SpecialConvertersTable
                .Where(entry => entry.FromType == fromType && entry.ToType == toType)
                .Select(entry => entry.Method)
                .FirstOrDefault();
        }

        private static MethodInfo GetConversionOperatorMethodInfo(Type fromType, Type toType)
        {
            MethodInfo[] methods = fromType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            MethodInfo conversionOperatorMethod = GetConversionOperatorMethodInfo(fromType, toType, methods);
            if (conversionOperatorMethod != null) return conversionOperatorMethod;
            methods = toType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            return GetConversionOperatorMethodInfo(fromType, toType, methods);
        }

        private static MethodInfo GetConversionOperatorMethodInfo(Type fromType, Type toType, MethodInfo[] methods)
        {
            foreach (MethodInfo mi in methods)
            {
                if (mi.ReturnType == toType && (mi.Name == "op_Implicit" || mi.Name == "op_Explicit"))
                {
                    ParameterInfo[] pis = mi.GetParameters();
                    if (pis.Length == 1 && pis[0].ParameterType == fromType) return mi;
                }
            }
            return null;
        }

    }
}
