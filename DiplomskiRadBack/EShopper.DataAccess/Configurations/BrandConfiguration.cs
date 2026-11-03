using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class BrandConfiguration : EntityConfiguration<Brand>
    {
        public override void ConfigureRules(EntityTypeBuilder<Brand> builder)
        {
            builder.Property(x => x.BrandName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Src).IsRequired();
            builder.Property(x => x.Alt).IsRequired().HasMaxLength(50);

            builder.HasMany(x => x.Products).WithOne(x => x.Brand).HasForeignKey(x => x.BrandId);
        }
    }
}
