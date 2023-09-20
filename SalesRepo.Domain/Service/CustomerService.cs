using Azure.Core;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SalesRepo.Data.Models;
using SalesRepo.Domain.Exceptions;
using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;

namespace SalesRepo.Domain.Service;

public class CustomerService : ICustomerService
{
    private readonly Func<SalesRepoContext> _contextFactory;
    private readonly IValidator<CreateCustomerRequest> _createCustomerValidator;
    private readonly IValidator<UpdateCustomerRequest> _updateCustomerValidator;

    public CustomerService(Func<SalesRepoContext> contextFactory, IValidator<CreateCustomerRequest> createCustomerValidator, IValidator<UpdateCustomerRequest> updateCustomerValidator)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _createCustomerValidator = createCustomerValidator ?? throw new ArgumentNullException(nameof(createCustomerValidator));
        _updateCustomerValidator = updateCustomerValidator ?? throw new ArgumentNullException(nameof(updateCustomerValidator));
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request)
    {
        _createCustomerValidator.ValidateAndThrow(request);

        await using var context = _contextFactory();

        var customer = new Customer
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
        };

        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        return new CustomerDto
        {
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.Phone,
            Id = customer.Id,
        };
    }

    public async Task DeleteCustomerAsync(int id)
    {
        await using var context = _contextFactory();

        var deleted = await context.Customers.Where(a => a.Id == id).ExecuteDeleteAsync();

        if (deleted == 0)
        {
            throw new EntityNotFoundException<Customer>(id);
        }
    }

    public async Task<CustomerDto> GetCustomerAsync(int id)
    {
        await using var context = _contextFactory();

        return await context.Customers
            .Include(a => a.Orders)
            .Where(a => a.Id == id)
             .OrderBy(a => a.FirstName)
             .ThenBy(a => a.LastName)
             .Select(a => new CustomerDto
             {
                 Email = a.Email,
                 FirstName = a.FirstName,
                 LastName = a.LastName,
                 Phone = a.Phone,
                 Id = a.Id,
                 Orders = a.Orders.Select(b => new OrderDto
                 {
                     CreatedDate = b.CreatedDate,
                     UpdatedDate = b.UpdatedDate,
                     OrderStatus = b.Status.ToString(),
                     ProductId = b.ProductId,
                     CustomerId = b.CustomerId,
                 }).ToList(),
             }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException<Customer>(id);
    }

    public async Task<IList<CustomerDto>> GetCustomerListAsync(string? search)
    {
        await using var context = _contextFactory();

        return await context.Customers
             .Include(a => a.Orders)
             .Where(a => search == null || a.FirstName.Contains(search) || a.LastName.Contains(search))
             .OrderBy(a => a.FirstName)
             .ThenBy(a => a.LastName)
             .Select(a => new CustomerDto
             {
                 Email = a.Email,
                 FirstName = a.FirstName,
                 LastName = a.LastName,
                 Phone = a.Phone,
                 Id = a.Id,
                 Orders = a.Orders.Select(b => new OrderDto
                 {
                     CreatedDate = b.CreatedDate,
                     UpdatedDate = b.UpdatedDate,
                     OrderStatus = b.Status.ToString(),
                     ProductId = b.ProductId,
                     CustomerId = b.CustomerId,
                 }).ToList(),
             }).ToListAsync();
    }

    public async Task<IList<OrderDto>> GetCustomerOrderListAsync(int id)
    {
        await using var context = _contextFactory();

        return await context.Orders
            .Include(a => a.Product)
            .Where(a => a.CustomerId == id)
            .Select(a =>
                new OrderDto
                {
                    CreatedDate = a.CreatedDate,
                    UpdatedDate = a.UpdatedDate,
                    OrderStatus = a.Status.ToString(),
                    ProductId = a.ProductId,
                    CustomerId = a.CustomerId,
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

    public async Task<CustomerDto> UpdateCustomerAsync(UpdateCustomerRequest request)
    {
        _updateCustomerValidator.ValidateAndThrow(request);

        await using var context = _contextFactory();

        var customer = await context.Customers.FirstOrDefaultAsync(a => a.Id == request.Id)
            ?? throw new EntityNotFoundException<Customer>(request.Id);
        
        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.Phone = request.Phone;

        await context.SaveChangesAsync();

        return new CustomerDto
        {
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.Phone,
            Id = customer.Id,
        };
    }
}
