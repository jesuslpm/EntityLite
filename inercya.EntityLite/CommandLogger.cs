using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace inercya.EntityLite
{

	public interface ICommandExecutionLogger
	{
		void LogCommandExecution(DbCommand command, DataService dataService, long executionTimeInMicroSeconds);
	}

	public static class CommandExecutionLogger
	{
		public static ICommandExecutionLogger Current { get; set; }

		public static void LogCommandExecution(DbCommand command, DataService dataService, long executionTimeInMicroSeconds)
		{
			if (Current != null)
			{
				Current.LogCommandExecution(command, dataService, executionTimeInMicroSeconds);
			}
		}
	}
}
