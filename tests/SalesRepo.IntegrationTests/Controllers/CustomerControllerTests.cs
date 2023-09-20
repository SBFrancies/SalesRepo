using Microsoft.EntityFrameworkCore;
using SalesRepo.Domain.Helpers;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;
using System.Net;
using System.Numerics;
using System.Text.Json;
using Xunit;

namespace SalesRepo.IntegrationTests.Controllers
{
    public class CustomerControllerTests
    {
        [Fact]
        public async Task CustomerController_PostAsync_CanCreateCustomer()
        {
            await using var context = new SalesRepoWebApplicationFactory();

            var client = context.CreateDefaultClient();
            var request = new CreateCustomerRequest
            {
                Email = "test@test.com",
                FirstName = "test-first",
                LastName = "test-last",
                Phone = "0123456789"
            };
            var response = await client.PostAsJsonAsync("/api/customer", request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CustomerDto>(json, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.FirstName, result.FirstName);
            Assert.Equal(request.LastName, result.LastName);
            Assert.Equal(request.Phone, result.Phone);
            Assert.NotEqual(default, result.Id);
            Assert.Empty(result.Orders);
        }
    }
}
