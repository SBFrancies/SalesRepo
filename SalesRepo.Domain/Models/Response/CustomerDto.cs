namespace SalesRepo.Domain.Models.Response;

public class CustomerDto
{
    public int Id { get; init; }

    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public string Phone { get; init; } = null!;

    public string Email { get; init; } = null!;

    public List<OrderDto> Orders { get; init; } = new();
}
