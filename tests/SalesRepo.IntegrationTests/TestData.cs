using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Payloads;
using SalesRepo.Data.Enums;
using SalesRepo.Data.Models;

namespace SalesRepo.IntegrationTests
{
    public static class TestData
    {
        public static List<Customer> Customers => new()
        {
            new()
            {
                Email = "test1",
                Phone = "test1",
                FirstName = "test1",
                LastName = "test1",
                Id = 1,
            },
            new()
            {
                Email = "test2",
                Phone = "test2",
                FirstName = "test2",
                LastName = "test2",
                Id = 2,
            },
            new()
            {
                Email = "test3",
                Phone = "test3",
                FirstName = "test3",
                LastName = "test3",
                Id = 3,
            }
        };

        public static List<Product> Products => new()
        {
            new()
            {
                Id = 1,
                Name = "test1",
                Description = "test1",
                Sku = "test1",
            },
            new()
            {
                Id = 2,
                Name = "test2",
                Description = "test2",
                Sku = "test2",
            },
            new()
            {
                Id = 3,
                Name = "test3",
                Description = "test3",
                Sku = "test3",
            },
        };

        public static List<Order> Orders => new()
        {
            new()
            {
                CustomerId = 1,
                ProductId = 1,
                Status = OrderStatus.Shipped,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,                
            },
            new()
            {
                CustomerId = 1,
                ProductId = 2,
                Status = OrderStatus.Pending,
                CreatedDate = DateTime.UtcNow,
            },
            new()
            {
                CustomerId = 2,
                ProductId = 1,
                Status = OrderStatus.Delivered,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
            }
        };
    }
}
