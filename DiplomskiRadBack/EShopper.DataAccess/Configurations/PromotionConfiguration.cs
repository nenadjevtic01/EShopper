using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class PromotionConfiguration : EntityConfiguration<Promotion>
    {
        public override void ConfigureRules(EntityTypeBuilder<Promotion> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            builder.HasMany(x => x.Groups).WithOne(x => x.Promotion).HasForeignKey(x => x.PromotionId);
        }
    }
}
