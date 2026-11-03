using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopper.Domain;

namespace EShopper.DataAccess.Configurations
{
    public class NewsletterSubscriberConfiguration : EntityConfiguration<NewsletterSubscriber>
    {
        public override void ConfigureRules(EntityTypeBuilder<NewsletterSubscriber> builder)
        {
            builder.Property(x => x.Email).IsRequired().HasMaxLength(100);

            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}
