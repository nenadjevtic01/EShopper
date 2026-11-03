using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class CouponConfiguration : EntityConfiguration<Coupon>
    {
        public override void ConfigureRules(EntityTypeBuilder<Coupon> builder)
        {
            builder.Property(x => x.Code).IsRequired().HasMaxLength(30);
            builder.Property(x => x.DiscountPercentage).IsRequired();
            builder.HasCheckConstraint("CK_Coupons_DiscountPercentage", "[DiscountPercentage] >= 1 AND [DiscountPercentage] <= 100");
            builder.Property(x => x.ValidUntil).IsRequired();

            builder.HasIndex(x => x.Code).IsUnique();

            builder.HasMany(x => x.Usages).WithOne(x => x.Coupon).HasForeignKey(x => x.CouponId);
        }
    }
}
