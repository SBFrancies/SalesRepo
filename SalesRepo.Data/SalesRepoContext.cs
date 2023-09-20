using Microsoft.EntityFrameworkCore;
using SalesRepo.Data.Enums;

namespace SalesRepo.Data.Models;

public class SalesRepoContext : DbContext
{
    public SalesRepoContext()
    {
    }

    public SalesRepoContext(DbContextOptions<SalesRepoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Customer");

            entity.HasIndex(e => e.Email, "UQ_Customer_Email").IsUnique();

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(40);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.CustomerId }).HasName("PK_Orders");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion(
            a => a.ToString(),
            a => (OrderStatus)Enum.Parse(typeof(OrderStatus), a));

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Orders_CustomerId");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Orders_ProductId");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Products");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Sku)
                .IsRequired()
                .HasMaxLength(40);
        });
    }
}