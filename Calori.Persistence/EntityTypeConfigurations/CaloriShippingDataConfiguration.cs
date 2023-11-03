using Calori.Domain.Models.CaloriAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class CaloriShippingDataConfiguration
        : IEntityTypeConfiguration<CaloriShippingData>
    {
        public void Configure(EntityTypeBuilder<CaloriShippingData> builder)
        {
            builder.HasKey(app => app.Id);
            builder.HasIndex(app => app.Id).IsUnique();
        }
    }
}