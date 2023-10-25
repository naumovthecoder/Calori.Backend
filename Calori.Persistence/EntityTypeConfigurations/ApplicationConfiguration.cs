using Calori.Domain.Models.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<CaloriApplication>
    {
        public void Configure(EntityTypeBuilder<CaloriApplication> builder)
        {
            builder.HasKey(app => app.Id);
            builder.HasIndex(app => app.Id).IsUnique();
            builder.Property(app => app.Email).HasMaxLength(256);

            builder.HasOne(app => app.ApplicationBodyParameters)
                .WithOne()
                .HasForeignKey<CaloriApplication>(app => app.ApplicationBodyParametersId);

            builder.HasMany(app => app.ApplicationAllergies)
                .WithOne()
                .HasForeignKey("ApplicationId");

            builder.Property(app => app.ApplicationAllergyId).HasColumnName("ApplicationAllergyId");

        }
    }
}