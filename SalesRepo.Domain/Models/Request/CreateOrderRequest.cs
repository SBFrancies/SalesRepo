namespace SalesRepo.Domain.Models.Request;

public class CreateOrderRequest
{
    public int CustomerId { get; init; }

    public int ProductID { get; init; }
}
