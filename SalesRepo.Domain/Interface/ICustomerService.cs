using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;

namespace SalesRepo.Domain.Interface;

public interface ICustomerService
{
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request);

    Task<CustomerDto> UpdateCustomerAsync(UpdateCustomerRequest request);

    Task<CustomerDto> GetCustomerAsync(int id);

    Task<IList<CustomerDto>> GetCustomerListAsync(string? search);

    Task<IList<OrderDto>> GetCustomerOrderListAsync(int id);

    Task DeleteCustomerAsync(int id);
}
