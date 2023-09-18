using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;

namespace SalesRepo.Domain.Interface;

public interface IProductService
{
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);

    Task<ProductDto> UpdateProductAsync(UpdateProductRequest request);

    Task<IList<ProductDto>> GetProductListAsync(string search);

    Task<IList<OrderDto>> GetProductOrderListAsync();

    Task DeleteProductAsync(int id);
}
