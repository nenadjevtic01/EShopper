using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class ProductConfiguration : EntityConfiguration<Product>
    {
        public override void ConfigureRules(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.ProductName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.CategoryId).IsRequired();
            builder.Property(x => x.BrandId).IsRequired();
            builder.Property(x => x.GenderId).IsRequired();
            builder.Property(x => x.CountryId).IsRequired();
            builder.Property(x => x.InStock).IsRequired();
            builder.Property(x => x.Material).IsRequired().HasMaxLength(50);

            builder.HasMany(x => x.Pictures).WithOne(x => x.Product).HasForeignKey(x => x.ProductId);
            builder.HasMany(x => x.ProductSizes).WithOne(x => x.Product).HasForeignKey(x => x.ProductId);
            builder.HasMany(x => x.CartItems).WithOne(x => x.Product).HasForeignKey(x => x.ProductId);
            builder.HasMany(x => x.ReceiptItems).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Prices).WithOne(x => x.Product).HasForeignKey(x => x.ProductId);
        }
    }
}
