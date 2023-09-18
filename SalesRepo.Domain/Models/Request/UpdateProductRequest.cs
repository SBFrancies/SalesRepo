namespace SalesRepo.Domain.Models.Request;

public class UpdateProductRequest
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;

    public string Sku { get; init; } = null!;
}
