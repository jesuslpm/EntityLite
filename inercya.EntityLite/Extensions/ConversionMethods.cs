/*
Copyright 2014 i-nercya intelligent software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json.Linq;

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
            },
            new ConverterEntry
            {
                FromType = typeof(string),
                ToType = typeof(JToken),
                Method = typeof(ConversionMethods).GetMethod("FromStringToJToken")
            }
        };

        public static JToken FromStringToJToken(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            return JToken.Parse(str);
        }

        public static JObject FromStringToJObject(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            return JObject.Parse(str);
        }

        public static DateTime FromDateTimeOffsetToLocalDateTime(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.LocalDateTime;
        }

        public static MethodInfo GetConversionMethod(Type fromType, Type toType)
        {
            MethodInfo converter = SpecialConvertersTable
                .Where(entry => entry.FromType == fromType && entry.ToType == toType)
                .Select(entry => entry.Method)
                .FirstOrDefault();
            if (converter != null) return converter;
            return GetConversionOperatorMethodInfo(fromType, toType);
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
