using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SalesRepo.Data.Models;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SalesRepo.IntegrationTests
{
    public class SalesRepoWebApplicationFactory : WebApplicationFactory<Api.Program>
    {
        private readonly bool _authorise;
        private readonly bool _seedData;

        public SalesRepoWebApplicationFactory(bool authorise = true, bool seedData = false) 
        {
            _authorise = authorise;
            _seedData = seedData;
        }

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
                    var connection = new SqliteConnection($"Data Source=:memory:");
                    connection.Open();
                    options.UseSqlite(connection);
                }, ServiceLifetime.Transient, ServiceLifetime.Transient);

                var realDbfactory = services.Single(a => a.ServiceType == typeof(Func<SalesRepoContext>));
                services.Remove(realDbfactory);
                services.AddSingleton<Func<SalesRepoContext>>(a => () =>
                {
                    var context = a.GetRequiredService<SalesRepoContext>();
                    context.Database.EnsureCreated();

                    if(_seedData)
                    {
                        SeedData(context);
                    }

                    return context;
                });

                if (_authorise)
                {
                    services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                                "Test", options => { });
                }
            });
        }

        private static void SeedData(SalesRepoContext context)
        {
            context.Customers.AddRange(TestData.Customers);
            context.Products.AddRange(TestData.Products);
            context.Orders.AddRange(TestData.Orders);
            context.SaveChanges();
        }
    }
}