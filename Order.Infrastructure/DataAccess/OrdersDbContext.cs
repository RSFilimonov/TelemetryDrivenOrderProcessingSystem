using Microsoft.EntityFrameworkCore;
using Order.Domain.Models;
using Order.Infrastructure.DataAccess.EntityTypeConfigurations;

namespace Order.Infrastructure.DataAccess;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext()
    {
            
    }

    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options) { }

    public DbSet<OrderModel> Orders { get; set; } = null!;
    public DbSet<OrderItemModel> OrderItems { get; set; } = null!;
    public DbSet<AddressModel> Addresses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(OrderConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(OrderItemConfiguration).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(AddressConfiguration).Assembly);

        base.OnModelCreating(builder);
    }
}