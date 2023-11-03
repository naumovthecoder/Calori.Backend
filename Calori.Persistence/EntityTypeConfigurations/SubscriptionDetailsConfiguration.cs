using Calori.Domain.Models.CaloriAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class SubscriptionDetailsConfiguration 
        : IEntityTypeConfiguration<SubscriptionDetails>
    {
        public void Configure(EntityTypeBuilder<SubscriptionDetails> builder)
        {
            builder.HasKey(app => app.Id);
            builder.HasIndex(app => app.Id).IsUnique();
        }
    }
}