using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;

namespace SalesRepo.Domain.Service;

public class OrderService : IOrderService
{
    public Task<OrderDto> CreateOrderAsync(int customerId, int productId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOrderAsync(int customerId, int productId)
    {
        throw new NotImplementedException();
    }

    public Task<IList<OrderDto>> GetOrderListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OrderDto> UpdateOrderAsync(UpdateOrderRequest request)
    {
        throw new NotImplementedException();
    }
}
