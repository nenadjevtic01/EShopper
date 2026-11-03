using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class CartConfiguration : EntityConfiguration<Cart>
    {
        public override void ConfigureRules(EntityTypeBuilder<Cart> builder)
        {
            builder.Property(x => x.UserId).IsRequired();

            builder.HasMany(x => x.Items).WithOne(x => x.Cart).HasForeignKey(x => x.CartId);
        }
    }
}
