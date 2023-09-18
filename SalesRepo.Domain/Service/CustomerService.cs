using SalesRepo.Domain.Interface;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Models.Response;

namespace SalesRepo.Domain.Service;

public class CustomerService : ICustomerService
{
    public Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCustomerAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IList<CustomerDto>> GetCustomerListAsync(string search)
    {
        throw new NotImplementedException();
    }

    public Task<IList<OrderDto>> GetCustomerOrderListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<CustomerDto> UpdateCustomerAsync(UpdateCustomerRequest request)
    {
        throw new NotImplementedException();
    }
}
