using CinemaTicketingSystem.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Infrastructure.Services
{
    public class TemporarySeatReservationCleanupService : BackgroundService
    {
        private readonly ILogger<TemporarySeatReservationCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public TemporarySeatReservationCleanupService(
            ILogger<TemporarySeatReservationCleanupService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CleanupReservationsService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a scope to resolve the scoped service
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var reservationRepository = scope.ServiceProvider.GetRequiredService<ITemporaryReservationRepository>();

                        // Calculate the cutoff time (10 minutes ago)
                        var cutoffTime = DateTime.UtcNow.AddMinutes(-10);

                        // Remove old records
                        await reservationRepository.RemoveOlderThanAsync(cutoffTime, stoppingToken);

                        _logger.LogInformation("Old temporary seat reservations cleaned up.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up temporary seat reservations.");
                }

                // Wait for 1 minute before running again
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("CleanupReservationsService is stopping.");
        }
    }
}

