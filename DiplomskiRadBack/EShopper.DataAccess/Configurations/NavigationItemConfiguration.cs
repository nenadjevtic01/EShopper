using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class NavigationItemConfiguration : EntityConfiguration<NavigationItem>
    {
        public override void ConfigureRules(EntityTypeBuilder<NavigationItem> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Path).IsRequired().HasMaxLength(200);
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.HasIndex(x => x.Path);
        }
    }
}
