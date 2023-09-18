using SalesRepo.Data.Enums;

namespace SalesRepo.Domain.Models.Request;

public class UpdateOrderRequest
{
    public int CustomerId { get; init; }

    public int ProductId { get; init; }

    public OrderStatus Status { get; init; }
}
