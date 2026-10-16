using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Raceday.Data;

namespace Raceday.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        // 🔥 Create this ONCE per factory, not per DbContext resolution
        private readonly string _databaseName = "RaceDayTestDb_" + Guid.NewGuid();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove all EF Core registrations
                var descriptorsToRemove = services
                    .Where(d =>
                        (d.ServiceType.FullName != null &&
                         d.ServiceType.FullName.Contains("EntityFrameworkCore")) ||
                        (d.ImplementationType?.FullName != null &&
                         d.ImplementationType.FullName.Contains("EntityFrameworkCore")) ||
                        (d.ServiceType == typeof(RaceDayDbContext)) ||
                        (d.ServiceType == typeof(DbContextOptions)) ||
                        (d.ServiceType == typeof(DbContextOptions<RaceDayDbContext>)))
                    .ToList();

                foreach (var descriptor in descriptorsToRemove)
                    services.Remove(descriptor);

                // Add In-Memory DbContext using the FIXED name
                services.AddDbContext<RaceDayDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);   // <-- uses the fixed name
                });

                // Ensure the database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<RaceDayDbContext>();
                db.Database.EnsureCreated();
            });
        }
    }
}