using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class CountryConfiguration : EntityConfiguration<Country>
    {
        public override void ConfigureRules(EntityTypeBuilder<Country> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Name);

            builder.HasMany(x => x.Products).WithOne(x => x.Country).HasForeignKey(x => x.CountryId);
        }
    }
}
