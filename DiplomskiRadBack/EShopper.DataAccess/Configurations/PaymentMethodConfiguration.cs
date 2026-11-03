using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class PaymentMethodConfiguration : EntityConfiguration<PaymentMethod>
    {
        public override void ConfigureRules(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Name);

            builder.HasMany(x => x.Receipts).WithOne(x => x.PaymentMethod).HasForeignKey(x => x.PaymentMethodId);
        }
    }
}
