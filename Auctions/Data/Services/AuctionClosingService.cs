using Auctions.Data.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auctions.Services
{
    public class AuctionClosingService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public AuctionClosingService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var listingsService = scope.ServiceProvider.GetRequiredService<IListingsService>();

                    try
                    {
                        await listingsService.CloseExpiredListings();
                        // Optionally handle success
                        Console.WriteLine($"Closed expired listings successfully at {DateTimeOffset.Now}");
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        Console.WriteLine($"Error occurred while closing expired listings: {ex.Message}");
                    }
                }

                // Adjust delay to 1 minute for more frequent execution
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }


    }
}
