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
    public class ProductControllerTests
    {
        [Fact]
        public async Task ProductController_PostAsync_CanCreateProduct()
        {
            await using var context = new SalesRepoWebApplicationFactory();

            var client = context.CreateDefaultClient();
            var request = new CreateProductRequest
            {
                Description = "test-description",
                Name = "test-name",
                Sku = "test-sku",
            };
            var response = await client.PostAsJsonAsync("/api/product", request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProductDto>(json, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Sku, result.Sku);
            Assert.NotEqual(default, result.Id);
            Assert.Empty(result.Orders);
        }

        [Fact]
        public async Task ProductController_PostAsync_BadRequestOnInvalidProduct()
        {
            await using var context = new SalesRepoWebApplicationFactory();

            var client = context.CreateDefaultClient();
            var request = new CreateProductRequest
            {
                Description = string.Empty,
                Name = string.Empty,
                Sku = "this-is-way-too-looooooooooooooooooooooooooooooooooooooooooooooooooooooooong",
            };

            var response = await client.PostAsJsonAsync("/api/product", request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var text = await response.Content.ReadAsStringAsync();

            Assert.NotNull(text);
            Assert.Contains("Validation failed", text);
        }

        [Fact]
        public async Task ProductController_DeleteAsync_CanDeleteProductWhoExists()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData:true);

            var client = context.CreateDefaultClient();

            var response = await client.DeleteAsync("/api/product/3");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task ProductController_DeleteAsync_NotFoundResponseWhenProductDoesNotExist()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: false);

            var client = context.CreateDefaultClient();

            var response = await client.DeleteAsync("/api/product/1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ProductController_GetAsync_CanGetAProduct()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData:true);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/product/1");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProductDto>(json, SerialisationSettings.DefaultOptions);
            var product = TestData.Products.First(a => a.Id == 1);

            Assert.NotNull(result);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Description, result.Description);
            Assert.Equal(product.Sku, result.Sku);
            Assert.Equal(product.Id, result.Id);
            Assert.NotEmpty(result.Orders);
        }


        [Fact]
        public async Task ProductController_GetAsync_NotFoundResponseWhenNoMatchingProduct()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: false);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/product/1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ProductController_GetAsync_CanGetAllProduct()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/product");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProductDto[]>(json, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(TestData.Products.Count, result.Length);
        }

        [Fact]
        public async Task ProductController_GetAsync_CanGetFilteredProduct()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/product?search=test1");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProductDto[]>(json, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task ProductController_GetOrdersAsync_CanGetOrderListForProduct()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var response = await client.GetAsync("/api/product/1/orders");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OrderDto[]>(json, SerialisationSettings.DefaultOptions);
            var orders = TestData.Orders.Where(a => a.ProductId == 1);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(orders.Count(), result.Length);
        }

        [Fact]
        public async Task ProductController_PutAsync_CanUpdateExistingProduct()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var request = new UpdateProductRequest
            {
                Description = "test-description-updated",
                Name = "test-name-updated",
                Sku = "test-sku-updated",
                Id = 1
            };

            var response = await client.PutAsJsonAsync("/api/product/1", request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProductDto>(json, SerialisationSettings.DefaultOptions);

            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Sku, result.Sku);
            Assert.Equal(request.Id, result.Id);
        }

        [Fact]
        public async Task ProductController_PutAsync_NotFoundResponseForNonExistantProduct()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var request = new UpdateProductRequest
            {
                Description = "test-description-updated",
                Name = "test-name-updated",
                Sku = "test-sku-updated",
                Id = 4
            };

            var response = await client.PutAsJsonAsync("/api/product/4", request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ProductController_PutAsync_BadRequestForInvalidUpdate()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var request = new UpdateProductRequest
            {
                Description = string.Empty,
                Name = string.Empty,
                Sku = "this-is-way-too-looooooooooooooooooooooooooooooooooooooooooooooooooooooooong",
                Id = 1,
            };

            var response = await client.PutAsJsonAsync("/api/product/1", request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var text = await response.Content.ReadAsStringAsync();

            Assert.NotNull(text);
            Assert.Contains("Validation failed", text);
        }

        [Fact]
        public async Task ProductController_PutAsync_BadRequestForMismatchingIds()
        {
            await using var context = new SalesRepoWebApplicationFactory(seedData: true);

            var client = context.CreateDefaultClient();

            var request = new UpdateProductRequest
            {
                Description = string.Empty,
                Name = string.Empty,
                Sku = "this-is-way-too-looooooooooooooooooooooooooooooooooooooooooooooooooooooooong",
                Id = 2,
            };

            var response = await client.PutAsJsonAsync("/api/product/1", request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var text = await response.Content.ReadAsStringAsync();

            Assert.NotNull(text);
            Assert.Contains("Route ID does not match", text);
        }
    }
}
