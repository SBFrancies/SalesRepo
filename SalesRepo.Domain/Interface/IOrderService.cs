using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;

namespace SalesRepo.Domain.Interface;

public interface IOrderService
{
    Task<OrderDto> UpdateOrderAsync(UpdateOrderRequest request);

    Task<IList<OrderDto>> GetOrderListAsync();

    Task<OrderDto> CreateOrderAsync(int customerId, int productId);

    Task DeleteOrderAsync(int customerId, int productId);
}
