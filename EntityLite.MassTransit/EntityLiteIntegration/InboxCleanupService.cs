namespace MassTransit.EntityLiteIntegration
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using inercya.EntityLite;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;


    /// <summary>
    /// The Inbox Cleanup Service is responsible for removing <see cref="InboxState" /> entries after the expiration
    /// window timeout has elapsed.
    /// </summary>
    /// <typeparam name="TDataService"></typeparam>
    public class InboxCleanupService<TDataService> :
        BackgroundService
        where TDataService : DataService
    {
        readonly ILogger<InboxCleanupService<TDataService>> _logger;
        readonly InboxCleanupServiceOptions _options;
        readonly IServiceProvider _provider;

        public InboxCleanupService(IOptions<InboxCleanupServiceOptions> options, ILogger<InboxCleanupService<TDataService>> logger,
            IServiceProvider provider)
        {
            _options = options.Value;
            _logger = logger;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanUpInboxState(stoppingToken).ConfigureAwait(false);
                    await Task.Delay(_options.QueryDelay, stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "CleanUpInboxState faulted");
                }
            }
        }

        async Task<int> CleanUpInboxState(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();
            using var ds = scope.ServiceProvider.GetRequiredService<TDataService>();
            var removeTimestamp = DateTime.UtcNow - _options.DuplicateDetectionWindow;
            var removed = await new QueryLite<InboxState>(Projection.BaseTable, ds)
                .Where(nameof(InboxState.Delivered), OperatorLite.Less, removeTimestamp)
                .DeleteAsync();
            _logger.LogDebug("Outbox Removed {Count} expired inbox messages", removed);
            return removed;
        }
    }
}
