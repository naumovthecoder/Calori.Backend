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
            
            // builder.HasOne(app => app.CaloriSlimmingPlan)
            //     .WithOne()
            //     .HasForeignKey<PersonalSlimmingPlan>(app => app.CaloriSlimmingPlanId);
        }
    }
}