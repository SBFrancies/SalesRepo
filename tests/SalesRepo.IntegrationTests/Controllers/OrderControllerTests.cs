using SalesRepo.Domain.Helpers;
using SalesRepo.Domain.Models.Response;
using System.Net;
using System.Text.Json;
using Xunit;

namespace SalesRepo.IntegrationTests.Controllers
{
    public class OrderControllerTests
    {
        [Fact]
        public async Task OrderController_GetAsync_ReturnsAllOrders()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/order");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OrderDto[]>(json, SerialisationSettings.DefaultOptions);
            var orders = TestData.Orders;

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(orders.Count, result.Length);
        }
    }
}
