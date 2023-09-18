namespace SalesRepo.Domain.Models.Response;

public class OrderDto
{
    public string OrderStatus { get; init; } = null!;

    public DateTimeOffset CreatedDate { get; init; }

    public DateTimeOffset? UpdatedDate { get; init; }

    public CustomerDto Customer { get; set; } = null!;

    public ProductDto Product { get; set; } = null!;
}
