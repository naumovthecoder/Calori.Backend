using Calori.Domain.Models.CaloriAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class CaloriSlimmingPlanConfiguration
        : IEntityTypeConfiguration<CaloriSlimmingPlan>
    {
        public void Configure(EntityTypeBuilder<CaloriSlimmingPlan> builder)
        {
            builder.HasKey(app => app.Id);
            builder.HasIndex(app => app.Id).IsUnique();
        }
    }
}