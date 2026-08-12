using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class AttendanceBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AttendanceBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var automation =
                    scope.ServiceProvider.GetRequiredService<IAttendanceAutomationService>();

                try
                {
                    await automation.MarkAbsentEmployeesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // Run every minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
