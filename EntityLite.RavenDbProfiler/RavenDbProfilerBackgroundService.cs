using inercya.EntityLite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace inercya.EntityLite.RavenDbProfiler
{
    internal class RavenDbProfilerBackgroundService : IHostedService
    {
        private readonly IDocumentStore documentStore;
        private readonly IServiceProvider serviceProvider;
        private readonly RavenDbProfilerSettings settings;

        private RavenDbProfilerLite profiler;

        public RavenDbProfilerBackgroundService(IDocumentStore documentStore, IOptions<RavenDbProfilerSettings> profilerOptions, IServiceProvider serviceProvider) 
        {
            this.documentStore = documentStore;
            this.serviceProvider = serviceProvider;
            this.settings = profilerOptions.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ILogger<RavenDbProfilerLite> logger = this.serviceProvider.GetRequiredService<ILogger<RavenDbProfilerLite>>();
            this.profiler = new RavenDbProfilerLite(this.documentStore, this.settings, logger);
            ConfigurationLite.Profiler = this.profiler;
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.profiler != null)
            {
                ConfigurationLite.Profiler = new NullProfilerLite();
                await this.profiler.DisposeAsync();
            }
        }
    }
}
