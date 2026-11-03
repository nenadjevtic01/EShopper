using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class CategoryConfiguration : EntityConfiguration<Category>
    {
        public override void ConfigureRules(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.CategoryName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Src).IsRequired();
            builder.Property(x => x.Alt).IsRequired().HasMaxLength(50);

            builder.HasMany(x => x.Products).WithOne(x => x.Category).HasForeignKey(x => x.CategoryId);
        }
    }
}
