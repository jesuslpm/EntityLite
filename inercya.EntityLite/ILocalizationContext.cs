using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
	public interface ICurrentLanguageService
	{
        string CurrentLanguageCode { get; }
		int GetCurrentLanguageIndex();
	}
}
