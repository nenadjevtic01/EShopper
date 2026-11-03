using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class PromotionGroupProductConfiguration : EntityConfiguration<PromotionGroupProduct>
    {
        public override void ConfigureRules(EntityTypeBuilder<PromotionGroupProduct> builder)
        {
            builder.Property(x => x.PromotionGroupId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.BasePrice).IsRequired().HasPrecision(6, 2);

            builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
