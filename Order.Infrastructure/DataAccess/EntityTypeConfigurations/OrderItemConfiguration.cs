using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Models;

namespace Order.Infrastructure.DataAccess.EntityTypeConfigurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItemModel>
{
    public void Configure(EntityTypeBuilder<OrderItemModel> builder)
    {
        builder.HasKey(order => order.Id);
    }
}