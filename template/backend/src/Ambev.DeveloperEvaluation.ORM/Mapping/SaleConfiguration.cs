using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SaleNumber).IsRequired();
        builder.Property(s => s.CustomerName).IsRequired();
        builder.Property(s => s.BranchName).IsRequired();
        builder.Property(s => s.Status).IsRequired();

        builder.HasMany(s => s.Items)
        .WithOne()
        .HasForeignKey("SaleId")
        .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}