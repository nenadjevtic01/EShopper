using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class CartItemConfiguration : EntityConfiguration<CartItem>
    {
        public override void ConfigureRules(EntityTypeBuilder<CartItem> builder)
        {
            builder.Property(x => x.CartId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.SizeId).IsRequired();

        }
    }
}
