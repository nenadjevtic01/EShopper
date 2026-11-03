using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class PromotionGroupConfiguration : EntityConfiguration<PromotionGroup>
    {
        public override void ConfigureRules(EntityTypeBuilder<PromotionGroup> builder)
        {
            builder.Property(x => x.PromotionId).IsRequired();
            builder.Property(x => x.DiscountPercentage).IsRequired();
            builder.HasCheckConstraint(
                "CK_PromotionGroups_DiscountPercentage",
                "[DiscountPercentage] >= 1 AND [DiscountPercentage] <= 100");

            builder.HasMany(x => x.Products).WithOne(x => x.PromotionGroup).HasForeignKey(x => x.PromotionGroupId);
        }
    }
}
