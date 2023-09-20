using Microsoft.EntityFrameworkCore;
using SalesRepo.Data.Enums;
using SalesRepo.Data.Models;
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
                Phone = "0123456789",
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

        [Fact]
        public async Task CustomerController_PostAsync_BadRequestOnInvalidCustomer()
        {
            await using var context = new SalesRepoWebApplicationFactory();

            var client = context.CreateDefaultClient();
            var request = new CreateCustomerRequest
            {
                Email = "not-a-valid-email",
                FirstName = string.Empty,
                LastName = string.Empty,
                Phone = string.Empty,
            };
            var response = await client.PostAsJsonAsync("/api/customer", request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var text = await response.Content.ReadAsStringAsync();

            Assert.NotNull(text);
            Assert.Contains("Validation failed", text);
        }

        [Fact]
        public async Task CustomerController_DeleteAsync_CanDeleteCustomerWhoExists()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData:true);

            var client = context.CreateDefaultClient();

            var response = await client.DeleteAsync("/api/customer/3");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task CustomerController_DeleteAsync_NotFoundResponseWhenCustomerDoesNotExist()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: false);

            var client = context.CreateDefaultClient();

            var response = await client.DeleteAsync("/api/customer/1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CustomerController_GetAsync_CanGetACustomer()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData:true);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/customer/1");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CustomerDto>(json, SerialisationSettings.DefaultOptions);
            var customer = TestData.Customers.First(a => a.Id == 1);

            Assert.NotNull(result);
            Assert.Equal(customer.Email, result.Email);
            Assert.Equal(customer.FirstName, result.FirstName);
            Assert.Equal(customer.LastName, result.LastName);
            Assert.Equal(customer.Phone, result.Phone);
            Assert.Equal(customer.Id, result.Id);
            Assert.NotEmpty(result.Orders);
        }


        [Fact]
        public async Task CustomerController_GetAsync_NotFoundResponseWhenNoMatchingCustomer()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: false);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/customer/1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CustomerController_GetAsync_CanGetAllCustomer()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/customer");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CustomerDto[]>(json, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(TestData.Customers.Count, result.Length);
        }

        [Fact]
        public async Task CustomerController_GetAsync_CanGetFilteredCustomer()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/customer?search=test1");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CustomerDto[]>(json, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task CustomerController_GetOrdersAsync_CanGetOrderListForCustomer()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/customer/1/orders");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OrderDto[]>(json, SerialisationSettings.DefaultOptions);
            var orders = TestData.Orders.Where(a => a.CustomerId == 1);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(orders.Count(), result.Length);
        }

        [Fact]
        public async Task CustomerController_PutAsync_CanUpdateExistingCustomer()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var request = new UpdateCustomerRequest
            {
                Email = "test@test.com",
                FirstName = "test-first",
                LastName = "test-last",
                Phone = "0123456789",
                Id = 1
            };

            var response = await client.PutAsJsonAsync("/api/customer/1", request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CustomerDto>(json, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.FirstName, result.FirstName);
            Assert.Equal(request.LastName, result.LastName);
            Assert.Equal(request.Phone, result.Phone);
            Assert.Equal(request.Id, result.Id);
        }

        [Fact]
        public async Task CustomerController_PutAsync_NotFoundResponseForNonExistantCustomer()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var request = new UpdateCustomerRequest
            {
                Email = "test@test.com",
                FirstName = "test-first",
                LastName = "test-last",
                Phone = "0123456789",
                Id = 4
            };

            var response = await client.PutAsJsonAsync("/api/customer/4", request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CustomerController_PutAsync_BadRequestForInvalidUpdate()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var request = new UpdateCustomerRequest
            {
                Email = "not-a-valid-email",
                FirstName = string.Empty,
                LastName = string.Empty,
                Phone = string.Empty,
                Id = 1
            };
            var response = await client.PutAsJsonAsync("/api/customer/1", request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var text = await response.Content.ReadAsStringAsync();

            Assert.NotNull(text);
            Assert.Contains("Validation failed", text);
        }

        [Fact]
        public async Task CustomerController_PutAsync_BadRequestForMismatchingIds()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var request = new UpdateCustomerRequest
            {
                Email = "not-a-valid-email",
                FirstName = string.Empty,
                LastName = string.Empty,
                Phone = string.Empty,
                Id = 2
            };
            var response = await client.PutAsJsonAsync("/api/customer/1", request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var text = await response.Content.ReadAsStringAsync();

            Assert.NotNull(text);
            Assert.Contains("Route ID does not match", text);
        }

        [Fact]
        public async Task CustomerController_PostOrderAsync_CanCreateCustomerOrder()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData:true);

            var client = context.CreateDefaultClient();

            var response = await client.PostAsync("/api/customer/1/order/3", null);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OrderDto>(json, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(1, result.CustomerId);
            Assert.Equal(3, result.ProductId);
            Assert.Equal(OrderStatus.Pending.ToString(), result.OrderStatus);
            Assert.NotEqual(DateTimeOffset.MinValue, result.CreatedDate);
            Assert.Null(result.UpdatedDate);
            Assert.NotNull(result.Customer);
            Assert.NotNull(result.Product);
        }

        [Fact]
        public async Task CustomerController_GetOrderAsync_CanGetCustomerOrder()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/customer/1/order/2");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OrderDto>(json, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(1, result.CustomerId);
            Assert.Equal(2, result.ProductId);
            Assert.Equal(OrderStatus.Pending.ToString(), result.OrderStatus);
            Assert.NotNull(result.Customer);
            Assert.NotNull(result.Product);
        }

        [Fact]
        public async Task CustomerController_DeleteOrderAsync_CanDeleteCustomerOrderExists()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var response = await client.DeleteAsync("/api/customer/1/order/1");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task CustomerController_DeleteOrderAsync_NotFoundResponseWhenCustomerOrderDoesNotExist()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: false);

            var client = context.CreateDefaultClient();

            var response = await client.DeleteAsync("/api/customer/1/order/3");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
