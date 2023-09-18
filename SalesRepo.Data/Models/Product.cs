namespace SalesRepo.Data.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}