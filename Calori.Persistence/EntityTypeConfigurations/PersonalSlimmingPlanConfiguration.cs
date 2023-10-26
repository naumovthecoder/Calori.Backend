using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.CaloriAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class PersonalSlimmingPlanConfiguration 
        : IEntityTypeConfiguration<PersonalSlimmingPlan>
    {
        public void Configure(EntityTypeBuilder<PersonalSlimmingPlan> builder)
        {
            builder.HasKey(app => app.Id);
            builder.HasIndex(app => app.Id).IsUnique();
        }
    }
}