using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SalesRepo.Data.Models;

namespace SalesRepo.IntegrationTests
{
    public class SalesRepoWebApplicationFactory : WebApplicationFactory<Api.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var realDb = services.Single(
                    a => a.ServiceType ==
                        typeof(DbContextOptions<SalesRepoContext>));

                services.Remove(realDb);

                services.AddDbContext<SalesRepoContext>(options =>
                {
                    options.UseInMemoryDatabase($"InMemoryDbForTesting-{Guid.NewGuid()}");
                }, ServiceLifetime.Transient, ServiceLifetime.Singleton);
            });
        }
    }
}
