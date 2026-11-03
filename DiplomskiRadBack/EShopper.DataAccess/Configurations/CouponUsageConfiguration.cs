using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class CouponUsageConfiguration : EntityConfiguration<CouponUsage>
    {
        public override void ConfigureRules(EntityTypeBuilder<CouponUsage> builder)
        {
            builder.Property(x => x.CouponId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.ReceiptId).IsRequired();

            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Receipt).WithMany().HasForeignKey(x => x.ReceiptId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
