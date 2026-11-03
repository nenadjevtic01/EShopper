using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class ContactMessageConfiguration : EntityConfiguration<ContactMessage>
    {
        public override void ConfigureRules(EntityTypeBuilder<ContactMessage> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Subject).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Message).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.Answer).IsRequired(false).HasMaxLength(1000);
            builder.Property(x => x.IsAnswered).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.AnsweredAt).IsRequired(false);
        }
    }
}
