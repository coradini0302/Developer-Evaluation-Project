using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SaleNumber).IsRequired();

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.CustomerName).IsRequired();

            builder.Property(x => x.BranchId).IsRequired();
            builder.Property(x => x.BranchName).IsRequired();

            builder.Ignore(x => x.Items);

            builder.HasMany(typeof(SaleItem), "_items")
                .WithOne()
                .HasForeignKey("SaleId");

            builder.Navigation("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
