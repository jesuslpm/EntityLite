using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using inercya.EntityLite.Extensions;

namespace inercya.EntityLite
{
	public static class ContextLite
	{
		public static ILocalizationContext LocalizationContext { get; set; }

		public static IIdentityMap SessionIdentityMap { get; set; }

		public static string GetSufixedLocalizedFieldName(string localizedFieldName)
		{
			if (LocalizationContext == null) return null;
			int index = LocalizationContext.GetCurrentLanguageIndex();
			if (index == -1) index = 0;
			return localizedFieldName + "_Lang" + (index + 1).ToString();
		}

		public static string GetLocalizedValue(object entity, string localizedFieldName)
		{
			if (LocalizationContext == null) return null;
			return (string)entity.GetPropertyValue(GetSufixedLocalizedFieldName(localizedFieldName));
		}
	}


}
