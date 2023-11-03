using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.CaloriAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class CaloriFeedbackConfiguration
        : IEntityTypeConfiguration<CaloriFeedback>
    {
        public void Configure(EntityTypeBuilder<CaloriFeedback> builder)
        {
            builder.HasKey(app => app.Id);
            builder.HasIndex(app => app.Id).IsUnique();
        }
    }
}