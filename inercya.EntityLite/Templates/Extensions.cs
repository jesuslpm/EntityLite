using inercya.EntityLite.Providers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using inercya.EntityLite.Extensions;
using System.Data;

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
            return parameterRegex.Replace(template.TransformText(),
                (match) => parameterPrefix + match.Value.Substring(2, match.Value.Length - 3));
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
                    parameter.DbType = parameterAttr.DbType;
                    parameter.Direction = parameterAttr.Direction;
                    if (parameterAttr.Size != 0) parameter.Size = parameterAttr.Size;
                    if (parameterAttr.Precision != 0) parameter.Precision = parameterAttr.Precision;
                    if (parameterAttr.Scale != 0) parameter.Scale = parameterAttr.Scale;
                    object propValue = getters[pi.Name](template);
                    parameter.Value = propValue == null ? DBNull.Value : propValue;
                    command.Parameters.Add(parameter);
                }
            }
        }
    }
}

