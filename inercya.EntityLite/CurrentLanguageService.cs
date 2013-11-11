using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using inercya.EntityLite;
using inercya.EntityLite.Extensions;

namespace inercya.EntityLite
{
    public static class CurrentLanguageService
    {

        private static string[] DefaultSupportedLanguages = { "en", "es" };

        public static string[] _supportedLanguages;


        public static string[] SupportedLanguages
        {
            get
            {
                if (_supportedLanguages == null)
                {
                    _supportedLanguages = DefaultSupportedLanguages;
                }
                return _supportedLanguages;
            }
            set
            {
                _supportedLanguages = value;
            }
        }

        public static string CurrentLanguageCode
        {
            get
            {
                var languageCode = Thread.CurrentThread.CurrentCulture.Name.Split('-')[0];
                var index = Array.IndexOf<string>(SupportedLanguages, languageCode);
                if (index < 0) return SupportedLanguages[0];
                return languageCode;
            }
        }

        public static int GetCurrentLanguageIndex()
        {
            var languageCode = Thread.CurrentThread.CurrentCulture.Name.Split('-')[0];
            var index = Array.IndexOf<string>(SupportedLanguages, languageCode);
            if (index < 0) return 0;
            return index;
        }

        public static string GetSufixedLocalizedFieldName(string localizedFieldName)
        {
            int index = GetCurrentLanguageIndex();
            return localizedFieldName + "_Lang" + (index + 1).ToString();
        }

        public static string GetLocalizedValue(object entity, string localizedFieldName)
        {
            return (string)entity.GetPropertyValue(GetSufixedLocalizedFieldName(localizedFieldName));
        }
    }
}
