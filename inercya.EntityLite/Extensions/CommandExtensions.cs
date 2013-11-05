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
