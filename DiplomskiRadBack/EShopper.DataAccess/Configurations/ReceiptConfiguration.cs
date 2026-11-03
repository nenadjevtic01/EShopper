using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class ReceiptConfiguration : EntityConfiguration<Receipt>
    {
        public override void ConfigureRules(EntityTypeBuilder<Receipt> builder)
        {
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.PaymentMethodId).IsRequired();
            builder.Property(x => x.Subtotal).IsRequired().HasPrecision(7, 2);
            builder.Property(x => x.ShippingFee).IsRequired().HasPrecision(6, 2);
            builder.Property(x => x.Total).IsRequired().HasPrecision(7, 2);

            builder.Property(x => x.BillingFirstName).IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.BillingLastName).IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.BillingEmail).IsRequired(false).HasMaxLength(100);
            builder.Property(x => x.BillingAddress).IsRequired(false).HasMaxLength(200);
            builder.Property(x => x.BillingCity).IsRequired(false).HasMaxLength(100);
            builder.Property(x => x.BillingCountry).IsRequired(false).HasMaxLength(100);
            builder.Property(x => x.BillingPostalCode).IsRequired(false).HasMaxLength(20);

            builder.Property(x => x.CouponId).IsRequired(false);
            builder.Property(x => x.DiscountAmount).IsRequired().HasPrecision(7, 2).HasDefaultValue(0m);

            builder
                .HasOne(x => x.Coupon)
                .WithMany()
                .HasForeignKey(x => x.CouponId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);

            builder.HasMany(x => x.ReceiptItems).WithOne(x => x.Receipt).HasForeignKey(x => x.ReceiptId);
        }
    }
}
