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
using System.Data.Common;

namespace inercya.EntityLite
{

	public interface IProfilerLite
	{
		void LogCommandExecution(DbCommand command, DataService dataService, TimeSpan executionTime);
	}

	public static class ProfilerLite
	{
		public static IProfilerLite Current { get; set; }

        public static void LogCommandExecution(DbCommand command, DataService dataService, TimeSpan executionTime)
		{
			if (Current != null)
			{
                Current.LogCommandExecution(command, dataService, executionTime);
			}
		}
	}
}
