using System;
using System.Collections.Generic;
using System.Text;

namespace inercya.EntityLite.RavenDbProfiler
{
    public class RavenDbProfilerSettings
    {
        public string DatabaseName { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(7);
    }
}
