using Calori.Domain.Models.CaloriAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class PaymentPeriodConfiguration
        : IEntityTypeConfiguration<PaymentPeriod>
    {
        public void Configure(EntityTypeBuilder<PaymentPeriod> builder)
        {
            builder.HasKey(app => app.Id);
            builder.HasIndex(app => app.Id).IsUnique();
        }
    }
}