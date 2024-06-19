using inercya.EntityLite.RavenDbProfiler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace inercya.EntityLite
{
    public static class RavenDbProfilerLiteConfigurationExtensions
    {
        public static IServiceCollection AddRavenDbProfilerLite(this IServiceCollection services, Action<RavenDbProfilerSettings> configure)
        {
            services.Configure(configure);
            services.AddHostedService<RavenDbProfilerBackgroundService>();
            return services;
        }
    }
}
