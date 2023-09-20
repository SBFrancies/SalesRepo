using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;

namespace SalesRepo.Domain.Interface;

public interface IProductService
{
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);

    Task<ProductDto> UpdateProductAsync(UpdateProductRequest request);

    Task<ProductDto> GetProductAsync(int id);

    Task<IList<ProductDto>> GetProductListAsync(string? search);

    Task<IList<OrderDto>> GetProductOrderListAsync(int id);

    Task DeleteProductAsync(int id);
}
