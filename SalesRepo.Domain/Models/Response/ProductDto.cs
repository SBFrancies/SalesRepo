namespace SalesRepo.Domain.Models.Response;

public class ProductDto
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;

    public string Sku { get; init; } = null!;

    public List<OrderDto> Orders { get; init; } = new();
}
