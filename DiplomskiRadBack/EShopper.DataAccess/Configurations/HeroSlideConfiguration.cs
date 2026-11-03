using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class HeroSlideConfiguration : EntityConfiguration<HeroSlide>
    {
        public override void ConfigureRules(EntityTypeBuilder<HeroSlide> builder)
        {
            builder.Property(x => x.Tagline).IsRequired(false).HasMaxLength(80);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(120);
            builder.Property(x => x.Subtitle).IsRequired(false).HasMaxLength(300);
            builder.Property(x => x.ButtonText).IsRequired(false).HasMaxLength(40);
            builder.Property(x => x.ButtonLink).IsRequired(false).HasMaxLength(200);
            builder.Property(x => x.MediaType).IsRequired().HasMaxLength(10);
            builder.Property(x => x.MediaSrc).IsRequired(false).HasMaxLength(300);
        }
    }
}
