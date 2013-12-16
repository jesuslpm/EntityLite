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
            StringBuilder sb = new StringBuilder(str.Length);
            bool isPreviousCharLowerCase = false;
            bool isNewWord = true;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == ' ' || c == '_')
                {
                    isNewWord = true;
                    continue;
                }
                bool isUpper = char.IsUpper(c);
                bool isLower = char.IsLower(c);
                if (isPreviousCharLowerCase && isUpper)
                {
                    isNewWord = true;
                }
                if (isNewWord)
                {
                    isNewWord = false;
                    if (isLower) c = char.ToUpper(c);
                }
                else
                {
                    if (isUpper) c = char.ToLower(c);
                }
                isPreviousCharLowerCase = isLower;
                sb.Append(c);
            }
            return sb.ToString();
        }


        public static string ToUnderscoreLowerCaseNamingConvention(this string str)
        {
            StringBuilder sb = new StringBuilder(str.Length + 6);
            bool isPreviousCharLowerCase = false;
            for (int i = 0; i < str.Length; i++ )
            {
                char c = str[i];
                bool isUpper = char.IsUpper(c);
                if (isPreviousCharLowerCase && isUpper)
                {
                    sb.Append('_');
                }
                if (c == ' ') c = '_';
                isPreviousCharLowerCase = char.IsLower(c);
                if (isUpper) c = char.ToLower(c);
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static string ToUnderscoreUpperCaseNamingConvention(this string str)
        {
            StringBuilder sb = new StringBuilder(str.Length + 8);
            bool isPreviousCharLowerCase = false;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                bool isLower = char.IsLower(c);
                if (isPreviousCharLowerCase && char.IsUpper(c))
                {
                    sb.Append('_');
                }
                if (c == ' ') c = '_';
                isPreviousCharLowerCase = isLower;
                if (isLower) c = char.ToUpper(c);
                sb.Append(c);
            }
            return sb.ToString();
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

        public static string Transform(this string str, TextTransform textTransform)
        {
            switch (textTransform)
            {
                case TextTransform.None:
                    return str;
                case TextTransform.ToLower:
                    return str.ToLower();
                case TextTransform.ToUpper:
                    return str.ToUpper();
                case TextTransform.ToPascalNamingConvention:
                    return str.ToPascalNamingConvention();
                case TextTransform.ToUnderscoreLowerCaseNamingConvention:
                    return str.ToUnderscoreLowerCaseNamingConvention();
                case TextTransform.ToUnderscoreUpperCaseNamingConvention:
                    return str.ToUnderscoreUpperCaseNamingConvention();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
