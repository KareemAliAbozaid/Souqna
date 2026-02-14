using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Souqna.Domin.Entities;

namespace Souqna.Infrastructure.Data.Config
{
    public class ProductCofiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(500);
            builder.Property(p => p.Description).HasMaxLength(2000);
            builder.Property(p => p.NewPrice).IsRequired().HasColumnType("decimal(18,2)");
        }
    }
}
