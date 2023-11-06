using Calori.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class CaloriStripeCustomerConfiguration
        : IEntityTypeConfiguration<CaloriStripeCustomer>
    {
        public void Configure(EntityTypeBuilder<CaloriStripeCustomer> builder)
        {
            builder.HasKey(app => app.CaloriUserId);
            builder.HasIndex(app => app.CaloriUserId).IsUnique();
        }
    }
}