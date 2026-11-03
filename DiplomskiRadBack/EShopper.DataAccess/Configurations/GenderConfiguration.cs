using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class GenderConfiguration : EntityConfiguration<Gender>
    {
        public override void ConfigureRules(EntityTypeBuilder<Gender> builder)
        {
            builder.Property(x => x.GenderName).IsRequired().HasMaxLength(50);

            builder.HasMany(x => x.Products).WithOne(x => x.Gender).HasForeignKey(x => x.GenderId);
        }
    }
}
