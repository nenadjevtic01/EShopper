using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class AuditLogConfiguration : EntityConfiguration<AuditLog>
    {
        public override void ConfigureRules(EntityTypeBuilder<AuditLog> builder)
        {
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}
