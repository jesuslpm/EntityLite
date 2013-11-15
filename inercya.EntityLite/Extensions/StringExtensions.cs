using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Extensions
{
    public static class StringExtensions
    {
        public static string ToPascalNamingConvention(this string str)
        {
            if (str == null) return null;
            var tokens = str.Split('_');
            string result = null;
            for (int i = 0; i < tokens.Length; i++)
            {
                string word;
                var token = tokens[i];
                if (token.HasUpperAndLowerCaseChars())
                {
                    word = token;
                }
                else
                {
                    if (token.Length > 1)
                    {
                        word = token.Substring(0, 1).ToUpper() + token.Substring(1).ToLower();
                    }
                    else if (token.Length == 1)
                    {
                        word = token.Substring(0, 1).ToUpper();
                    }
                    else
                    {
                        word = string.Empty;
                    }
                }
                if (i == 0)
                {
                    result = word;
                }
                else
                {
                    result += word;
                }
            }
            return result;
        }

        public static bool HasUpperAndLowerCaseChars(this string str)
        {
            bool hasLower = false;
            bool hasUpper = false;
            foreach(char c in str)
            {
                if (!hasLower && char.IsLower(c)) hasLower = true;
                if (!hasUpper && char.IsUpper(c)) hasUpper = true;
                if (hasLower && hasUpper) return true;
            }
            return false;
        }
    }
}
