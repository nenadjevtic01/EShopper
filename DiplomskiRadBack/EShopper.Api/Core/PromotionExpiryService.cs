using EShopper.DataAccess;
using EShopper.Implementation.UseCases;

namespace EShopper.Api.Core
{
    public class PromotionExpiryService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PromotionExpiryService> _logger;

        public PromotionExpiryService(IServiceScopeFactory scopeFactory, ILogger<PromotionExpiryService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<EShopperDbContext>();
                    var count = PromotionExpiry.DeactivateExpired(context, DateTime.UtcNow);

                    if (count > 0)
                    {
                        _logger.LogInformation("Deactivated {Count} expired promotion(s).", count);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Could not deactivate expired promotions.");
                }

                try
                {
                    await Task.Delay(Interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}
