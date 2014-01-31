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
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace inercya.EntityLite.Extensions
{
	public static class CommandExtensions
	{
		public static IDbDataParameter AddWithValue(this DbParameterCollection parameters, string parameterName, object parameterValue)
		{
			if (parameters == null) throw new ArgumentNullException("parameters");
			if (parameterValue == null) parameterValue = DBNull.Value;
			SqlParameterCollection sqlParameters = parameters as SqlParameterCollection;
			if (sqlParameters != null)
			{
				return sqlParameters.AddWithValue(parameterName, parameterValue);
			}
			var addWithValueMethod = parameters.GetType().GetMethod("AddWithValue");
			if (addWithValueMethod == null)
			{
				throw new NotImplementedException(string.Format("{0} does not implement AddWithValue method", parameters.GetType().Name));
			}
			return (IDbDataParameter)addWithValueMethod.Invoke(parameters, new object[] { parameterName, parameterValue });

		}

	}
}
