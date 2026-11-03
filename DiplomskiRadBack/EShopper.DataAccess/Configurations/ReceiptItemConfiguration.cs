using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class ReceiptItemConfiguration : EntityConfiguration<ReceiptItem>
    {
        public override void ConfigureRules(EntityTypeBuilder<ReceiptItem> builder)
        {
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.PriceId).IsRequired();
            builder.Property(x => x.SizeId).IsRequired();
            builder.Property(x => x.Quantity).IsRequired().HasMaxLength(3);
            builder.Property(x => x.ReceiptId).IsRequired();
        }
    }
}
