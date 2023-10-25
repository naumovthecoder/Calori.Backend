using Calori.Domain.Models.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class ApplicationAllergyConfiguration : IEntityTypeConfiguration<ApplicationAllergy>
    {
        public void Configure(EntityTypeBuilder<ApplicationAllergy> builder)
        {
            builder.HasKey(aa => aa.Id);
        
            builder.HasOne(aa => aa.Application)
                .WithMany(app => app.ApplicationAllergies)
                .HasForeignKey(aa => aa.ApplicationId);
        
            builder.Property(aa => aa.Allergy)
                .IsRequired();

            builder.ToTable("ApplicationAllergy");
        }
    }
}
