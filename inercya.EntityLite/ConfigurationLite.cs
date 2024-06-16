using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
//using Microsoft.Extensions.Logging.Abstractions;

namespace inercya.EntityLite
{
    public static class ConfigurationLite
    {
        static ConfigurationLite()
        {
            Profiler = new NullProfilerLite();
            LoggerFactory = new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory();
        }

        public static IProfilerLite Profiler { get; set; } 
        public static ILoggerFactory LoggerFactory { get; set; }
    }
}
