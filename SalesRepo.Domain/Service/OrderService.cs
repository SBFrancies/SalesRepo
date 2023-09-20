using Azure.Core;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SalesRepo.Data.Enums;
using SalesRepo.Data.Models;
using SalesRepo.Domain.Exceptions;
using SalesRepo.Domain.Helpers;
using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;

namespace SalesRepo.Domain.Service;

public class OrderService : IOrderService
{
    private readonly Func<SalesRepoContext> _contextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OrderService(Func<SalesRepoContext> contextFactory, IDateTimeProvider dateTimeProvider)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public async Task<OrderDto> CreateOrderAsync(int customerId, int productId)
    {
        await using var context = _contextFactory();

        var order = new Order
        {
            CreatedDate = _dateTimeProvider.UtcNow,
            CustomerId = customerId,
            ProductId = productId,
            Status = OrderStatus.Pending,
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        return await context.Orders
           .Include(a => a.Customer)
           .Include(a => a.Product)
           .Where(a => a.CustomerId == customerId && a.ProductId == productId)
           .Select(a =>
               new OrderDto
               {
                   CreatedDate = a.CreatedDate,
                   UpdatedDate = a.UpdatedDate,
                   OrderStatus = a.Status.ToString(),
                   ProductId = a.ProductId,
                   CustomerId = a.CustomerId,
                   Customer = new CustomerDto
                   {
                       Email = a.Customer.Email,
                       FirstName = a.Customer.FirstName,
                       LastName = a.Customer.LastName,
                       Phone = a.Customer.Phone,
                       Id = a.CustomerId,
                   },
                   Product = new ProductDto
                   {
                       Description = a.Product.Description,
                       Id = a.ProductId,
                       Name = a.Product.Name,
                       Sku = a.Product.Sku,
                   },
               }
           ).FirstAsync();
    }

    public async Task DeleteOrderAsync(int customerId, int productId)
    {
        await using var context = _contextFactory();

        var deleted = await context.Orders.Where(a => a.CustomerId == customerId && a.ProductId == productId).ExecuteDeleteAsync();

        if (deleted == 0)
        {
            throw new EntityNotFoundException<Customer>(customerId, productId);
        }
    }

    public async Task<OrderDto> GetOrderAsync(int customerId, int productId)
    {
        await using var context = _contextFactory();

        return await context.Orders
            .Include(a => a.Customer)
            .Include(a => a.Product)
            .Where(a => a.CustomerId == customerId && a.ProductId == productId)
            .Select(a =>
                new OrderDto
                {
                    CreatedDate = a.CreatedDate,
                    UpdatedDate = a.UpdatedDate,
                    OrderStatus = a.Status.ToString(),
                    ProductId = a.ProductId,
                    CustomerId = a.CustomerId,
                    Customer = new CustomerDto
                    {
                        Email = a.Customer.Email,
                        FirstName = a.Customer.FirstName,
                        LastName = a.Customer.LastName,
                        Phone = a.Customer.Phone,
                        Id = a.CustomerId,
                    },
                    Product = new ProductDto
                    {
                        Description = a.Product.Description,
                        Id = a.ProductId,
                        Name = a.Product.Name,
                        Sku = a.Product.Sku,
                    },
                }
        ).FirstOrDefaultAsync() ?? throw new EntityNotFoundException<Order>(customerId, productId);
    }

    public async Task<IList<OrderDto>> GetOrderListAsync()
    {
        await using var context = _contextFactory();

        return await context.Orders
            .Include(a => a.Customer)
            .Include(a => a.Product)
            .Select(a =>
                new OrderDto
                {
                    CreatedDate = a.CreatedDate,
                    UpdatedDate = a.UpdatedDate,
                    OrderStatus = a.Status.ToString(),
                    ProductId = a.ProductId,
                    CustomerId = a.CustomerId,
                    Customer = new CustomerDto
                    {
                        Email = a.Customer.Email,
                        FirstName = a.Customer.FirstName,
                        LastName = a.Customer.LastName,
                        Phone = a.Customer.Phone,
                        Id = a.CustomerId,
                    },
                    Product = new ProductDto
                    {
                        Description = a.Product.Description,
                        Id = a.ProductId,
                        Name = a.Product.Name,
                        Sku = a.Product.Sku,
                    },
                }
            )
            .ToListAsync();
    }

    public async Task<OrderDto> UpdateOrderAsync(UpdateOrderRequest request)
    {
        await using var context = _contextFactory();

        var order = await context.Orders.FirstOrDefaultAsync(a => a.CustomerId == request.CustomerId && a.ProductId == request.ProductId)
             ?? throw new EntityNotFoundException<Order>(request.CustomerId, request.ProductId);

        var validUpdates = OrderStatusUpdates.ValidUpdates[order.Status];

        if(!validUpdates.Contains(request.Status))
        {
            throw new InvalidUpdateException<OrderStatus>(order.Status, request.Status, nameof(Order.Status));
        }

        order.Status = request.Status;
        order.UpdatedDate = _dateTimeProvider.UtcNow;

        return await context.Orders
           .Include(a => a.Customer)
           .Include(a => a.Product)
           .Where(a => a.CustomerId == request.CustomerId && a.ProductId == request.ProductId)
           .Select(a =>
               new OrderDto
               {
                   CreatedDate = a.CreatedDate,
                   UpdatedDate = a.UpdatedDate,
                   OrderStatus = a.Status.ToString(),
                   ProductId = a.ProductId,
                   CustomerId = a.CustomerId,
                   Customer = new CustomerDto
                   {
                       Email = a.Customer.Email,
                       FirstName = a.Customer.FirstName,
                       LastName = a.Customer.LastName,
                       Phone = a.Customer.Phone,
                       Id = a.CustomerId,
                   },
                   Product = new ProductDto
                   {
                       Description = a.Product.Description,
                       Id = a.ProductId,
                       Name = a.Product.Name,
                       Sku = a.Product.Sku,
                   },
               }
           ).FirstAsync();
    }
}
