using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;

namespace SalesRepo.Domain.Service;

public class ProductService : IProductService
{
    public Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteProductAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IList<ProductDto>> GetProductListAsync(string search)
    {
        throw new NotImplementedException();
    }

    public Task<IList<OrderDto>> GetProductOrderListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ProductDto> UpdateProductAsync(UpdateProductRequest request)
    {
        throw new NotImplementedException();
    }
}
