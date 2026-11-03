using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class UseCaseConfiguration : EntityConfiguration<UseCase>
    {
        public override void ConfigureRules(EntityTypeBuilder<UseCase> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(50);

            builder.HasMany(x => x.UserUseCases).WithOne(x => x.UseCase).HasForeignKey(x => x.UseCaseId);
        }
    }
}
