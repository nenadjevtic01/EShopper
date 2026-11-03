using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class InfoConfiguration : EntityConfiguration<Info>
    {
        public override void ConfigureRules(EntityTypeBuilder<Info> builder)
        {
            builder.Property(x => x.Address).IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.City).IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.PostalCode).IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.Country).IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.UserId).IsRequired();

            builder.HasIndex(x => x.PostalCode);
            builder.HasIndex(x => x.Country);
        }
    }
}
