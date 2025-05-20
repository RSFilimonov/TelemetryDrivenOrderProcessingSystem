using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Models;

namespace Order.Infrastructure.DataAccess.EntityTypeConfigurations;

public class AddressConfiguration : IEntityTypeConfiguration<AddressModel>
{
    public void Configure(EntityTypeBuilder<AddressModel> builder)
    {
        builder.HasKey(order => order.Id);

        builder.HasMany(address => address.Orders)
            .WithOne(order => order.ShippingAddress)
            .HasForeignKey(order => order.ShippingAddressId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}