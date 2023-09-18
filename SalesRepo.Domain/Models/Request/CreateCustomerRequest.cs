namespace SalesRepo.Domain.Models.Request;

public class CreateCustomerRequest
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public string Phone { get; init; } = null!;

    public string Email { get; init; } = null!;
}
