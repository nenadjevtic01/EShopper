using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class ProductRatingConfiguration : EntityConfiguration<ProductRating>
    {
        public override void ConfigureRules(EntityTypeBuilder<ProductRating> builder)
        {
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Rating).IsRequired().HasPrecision(2, 1);
            builder.HasCheckConstraint("CK_ProductRatings_Rating", "[Rating] >= 1 AND [Rating] <= 5");
            builder.Property(x => x.Comment).IsRequired(false).HasMaxLength(1000);

            builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ProductId, x.UserId }).IsUnique();
        }
    }
}
