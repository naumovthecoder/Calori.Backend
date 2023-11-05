using Calori.Domain.Models.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class CaloriPriceConfiguration
        : IEntityTypeConfiguration<CaloriPrice>
    {
        public void Configure(EntityTypeBuilder<CaloriPrice> builder)
        {
            builder.HasKey(app => app.PriceId);
            builder.HasIndex(app => app.PriceId).IsUnique();
        }
    }
}