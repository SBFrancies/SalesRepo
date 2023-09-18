using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;

namespace SalesRepo.Domain.Interface;

public interface ICustomerService
{
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request);

    Task<CustomerDto> UpdateCustomerAsync(UpdateCustomerRequest request);

    Task<IList<CustomerDto>> GetCustomerListAsync(string search);

    Task<IList<OrderDto>> GetCustomerOrderListAsync();

    Task DeleteCustomerAsync(int id);
}
