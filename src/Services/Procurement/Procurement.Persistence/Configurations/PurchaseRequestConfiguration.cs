using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Procurement.Domain.PurchaseRequests.Aggregates;
using Procurement.Domain.PurchaseRequests.ValueObjects;

namespace Procurement.Persistence.Configurations;

public sealed class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
    {
        builder.ToTable("PurchaseRequests");

        builder.HasKey(request => request.Id);
        builder.Property(request => request.Id)
            .HasConversion(id => id.Value, value => PurchaseRequestId.From(value))
            .ValueGeneratedNever();

        builder.Property(request => request.RequesterId).IsRequired();
        builder.Property(request => request.DepartmentId).IsRequired();
        builder.Property(request => request.ApproverId).IsRequired();
        builder.Property(request => request.Status).HasConversion<string>().IsRequired();

        builder.Navigation(request => request.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.OwnsMany(request => request.Items, items =>
        {
            items.ToTable("PurchaseRequestItems");
            items.Property(item => item.Id)
                .HasConversion(id => id.Value, value => PurchaseRequestItemId.From(value));
            items.Property(item => item.Description).IsRequired();
            items.Property(item => item.Quantity)
                .HasConversion(quantity => quantity.Value, value => Quantity.From(value));
        });
    }
}
