using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class PictureConfiguration : EntityConfiguration<Picture>
    {
        public override void ConfigureRules(EntityTypeBuilder<Picture> builder)
        {
            builder.Property(x => x.Src).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Alt).IsRequired().HasMaxLength(128);
            builder.Property(x => x.ProductId).IsRequired();
        }
    }
}
