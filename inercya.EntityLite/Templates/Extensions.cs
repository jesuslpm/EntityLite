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

using inercya.EntityLite.Providers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using inercya.EntityLite.Extensions;
using System.Data;
using System.Collections;

namespace inercya.EntityLite.Templates
{
    public static class Extensions
    {

        private static readonly HashSet<string> Keywords = new HashSet<string>
		{
			"abstract", "add", "alias", "as", "ascending", "async", "await", "base", 
			"bool", "break", "byte", "case", "catch", "char", "checked", "class", 
			"const", "continue", "decimal", "default", "delegate", "descending",
			"do", "double", "dynamic", "else", "enum", "event", "explicit", "extern", 
			"false", "finally", "fixed", "float", "for", "foreach", "from", "get", 
			"global", "goto", "group", "if", "implicit", "into","in", "int", 
			"interface", "internal", "is", "join", "let", "lock", "long", "namespace", 
			"new", "null", "object", "operator", "out", "orderby", "override", "params", 
			"partial", "private", "protected", "public", "readonly", "ref",  "remove", 
			"return", "sbyte", "sealed", "select", "set", "short", "sizeof", "stackalloc", 
			"static", "string", "struct", "switch", "this", "throw", "true", "try", 
			"typeof", "uint", "ulong", "unckecked", "unsafe", "ushort", "using", 
			"value", "var", "where", "virtual", "void", "volatile", "while", "yield"
		};

        public static string EscapeKeyword(this string str)
        {
            if (Keywords.Contains(str))
            {
                return "@" + str;
            }
            else
            {
                return str;
            }
        }

        public static string ActualName(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;
            string genericTypeName = type.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                type.GetGenericArguments()
                    .Select(ta => ActualName(ta)).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }

        public static bool IsNullableValueType(this Type type)
        {
            return (type.IsGenericType && type.IsValueType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }


        public static string ToParameterName(this string self)
        {
            if (string.IsNullOrEmpty(self)) throw new ArgumentException("parameter cannot be null nor empty", "self");

            string firstCharacter = self.Substring(0, 1).ToLower();
            if (self.Length > 1)
            {
                return firstCharacter + self.Substring(1);
            }
            else
            {
                return firstCharacter;
            }
        }


        static readonly Regex parameterRegex = new Regex(@"\$\((\S)+\)", RegexOptions.Compiled);

        internal static string GetSql(this ISqlTemplate template, string parameterPrefix)
        {
            var sql = parameterRegex.Replace(template.TransformText(),
                (match) => parameterPrefix + match.Value.Substring(2, match.Value.Length - 3));

            return sql;
        }

        internal static void AddParametersToCommand(this ISqlTemplate template, DbCommand command, string parameterPrefix)
        {
            if (template == null) throw new ArgumentNullException("template");
            if (command == null) throw new ArgumentNullException("command");
            Type templateType = template.GetType();
            var getters = templateType.GetPropertyGetters();
            foreach (var pi in templateType.GetProperties())
            {
                var attrs = pi.GetCustomAttributes(typeof(DbParameterAttribute), false);
                if (attrs.Length > 0)
                {
                    var parameterAttr = (DbParameterAttribute)attrs[0];
                    IDbDataParameter parameter = command.CreateParameter();
                    parameter.ParameterName = parameterPrefix + pi.Name;
                    parameter.SourceColumn = pi.Name;
                    parameter.DbType = parameterAttr.DbType;
                    parameter.Direction = parameterAttr.Direction;
                    if (parameterAttr.Size != 0) parameter.Size = parameterAttr.Size;
                    if (parameterAttr.Precision != 0) parameter.Precision = parameterAttr.Precision;
                    if (parameterAttr.Scale != 0) parameter.Scale = parameterAttr.Scale;
                    object propValue = getters[pi.Name](template);
                    parameter.Value = propValue == null ? DBNull.Value : propValue;
                    command.Parameters.Add(parameter);
                }

                attrs = pi.GetCustomAttributes(typeof(DbParameterSerieAttribute), false);
                if (attrs.Length > 0)
                {
                    var parameterAttr = (DbParameterSerieAttribute)attrs[0];
                    object propValue = getters[pi.Name](template);
                    if (propValue != null)
                    {
                        var items = propValue as IEnumerable;
                        if (items == null)
                        {
                            throw new ArgumentException(pi.Name + " property is not enumerable. Properties decorated with DbParameterSerieAttribute must be enumerable");
                        }
                        int i = 0;
                        foreach (object item in items)
                        {
                            IDbDataParameter parameter = command.CreateParameter();
                            parameter.ParameterName = parameterPrefix + pi.Name + i.ToString();
                            parameter.SourceColumn = pi.Name + i.ToString();
                            parameter.DbType = parameterAttr.DbType;
                            parameter.Direction = parameterAttr.Direction;
                            if (parameterAttr.Size != 0) parameter.Size = parameterAttr.Size;
                            if (parameterAttr.Precision != 0) parameter.Precision = parameterAttr.Precision;
                            if (parameterAttr.Scale != 0) parameter.Scale = parameterAttr.Scale;

                            parameter.Value = item == null ? DBNull.Value : item;
                            command.Parameters.Add(parameter);
                            i++;
                        }
                    }
                }

            }
        }
    }
}

