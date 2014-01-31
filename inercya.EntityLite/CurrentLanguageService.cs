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
            return localizedFieldName + "Lang" + (index + 1).ToString();
        }

        public static string GetLocalizedValue(object entity, string localizedFieldName)
        {
            return (string)entity.GetPropertyValue(GetSufixedLocalizedFieldName(localizedFieldName));
        }
    }
}
