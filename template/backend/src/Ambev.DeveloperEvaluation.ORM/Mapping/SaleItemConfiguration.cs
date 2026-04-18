using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.ProductName).IsRequired();

            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.UnitPrice).IsRequired();

            builder.Property(x => x.Discount).IsRequired();
            builder.Property(x => x.TotalAmount).IsRequired();
        }
    }
}
