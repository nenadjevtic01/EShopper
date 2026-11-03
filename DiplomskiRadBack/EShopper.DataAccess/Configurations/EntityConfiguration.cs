using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T>
        where T : Entity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Id).UseIdentityColumn(1, 1);

            builder.Property(x => x.IsActive).IsRequired(true).HasDefaultValue(true);

            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            ConfigureRules(builder);
        }

        public abstract void ConfigureRules(EntityTypeBuilder<T> builder);
    }
}
