using Azure.Core;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SalesRepo.Data.Models;
using SalesRepo.Domain.Exceptions;
using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;
using SalesRepo.UnitTests.Validation;
using System.Numerics;

namespace SalesRepo.Domain.Service;

public class ProductService : IProductService
{
    private readonly Func<SalesRepoContext> _contextFactory;
    private readonly IValidator<CreateProductRequest> _createProductValidator;
    private readonly IValidator<UpdateProductRequest> _updateProductValidator;

    public ProductService(Func<SalesRepoContext> contextFactory, IValidator<CreateProductRequest> createProductValidator, IValidator<UpdateProductRequest> updateProductValidator)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _createProductValidator = createProductValidator ?? throw new ArgumentNullException(nameof(createProductValidator));
        _updateProductValidator = updateProductValidator ?? throw new ArgumentNullException(nameof(updateProductValidator));
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        _createProductValidator.ValidateAndThrow(request);

        await using var context = _contextFactory();

        var product = new Product
        {
            Description = request.Description,
            Name = request.Name,
            Sku = request.Sku,
        };

        await context.Products.AddAsync(product);

        await context.SaveChangesAsync();

        return new ProductDto
        {
            Description = product.Description,
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
        };
    }

    public async Task DeleteProductAsync(int id)
    {
        await using var context = _contextFactory();

        var deleted = await context.Products.Where(a => a.Id == id).ExecuteDeleteAsync();

        if(deleted == 0)
        {
            throw new EntityNotFoundException<Product>(id);
        }
    }

    public async Task<ProductDto> GetProductAsync(int id)
    {
        await using var context = _contextFactory();

        return await context.Products
             .Include(a => a.Orders)
             .Where(a => a.Id == id)
             .OrderBy(a => new { a.Name, a.Sku })
             .Select(a => new ProductDto
             {
                 Sku = a.Sku,
                 Name = a.Name,
                 Description = a.Description,
                 Id = a.Id,
                 Orders = a.Orders.Select(b => new OrderDto
                 {
                     CreatedDate = b.CreatedDate,
                     UpdatedDate = b.UpdatedDate,
                     OrderStatus = b.Status.ToString(),
                     ProductId = b.ProductId,
                     CustomerId = b.CustomerId,
                 }).ToList(),
             }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException<Product>(id);
    }

    public async Task<IList<ProductDto>> GetProductListAsync(string? search)
    {
        await using var context = _contextFactory();

        return await context.Products
             .Include(a => a.Orders)
             .Where(a => search == null || a.Name.Contains(search) || a.Sku.Contains(search))
             .OrderBy(a => a.Name)
             .ThenBy(a => a.Sku)
             .Select(a => new ProductDto
             {
                 Sku = a.Sku,
                 Name = a.Name,
                 Description = a.Description,
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

    public async Task<IList<OrderDto>> GetProductOrderListAsync(int id)
    {
        await using var context = _contextFactory();

        return await context.Orders
            .Include(a => a.Customer)
            .Where(a => a.ProductId == id)
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
                }
            )
            .ToListAsync();
    }

    public async Task<ProductDto> UpdateProductAsync(UpdateProductRequest request)
    {
        _updateProductValidator.ValidateAndThrow(request);

        await using var context = _contextFactory();

        var product = await context.Products.FirstOrDefaultAsync(a => a.Id == request.Id)
            ?? throw new EntityNotFoundException<Product>(request.Id);

        product.Sku = request.Sku;
        product.Name = request.Name;
        product.Description = request.Description;

        await context.SaveChangesAsync();

        return new ProductDto
        {
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            Id = product.Id,
        };
    }
}
