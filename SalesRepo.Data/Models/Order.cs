using SalesRepo.Data.Enums;

namespace SalesRepo.Data.Models;

public class Order
{
    public int ProductId { get; set; }

    public int CustomerId { get; set; }

    public OrderStatus Status { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? UpdatedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}