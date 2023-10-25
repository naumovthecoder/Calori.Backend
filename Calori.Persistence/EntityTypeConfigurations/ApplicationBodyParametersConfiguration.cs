using Calori.Domain.Models.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calori.Persistence.EntityTypeConfigurations
{
    public class ApplicationBodyParametersConfiguration 
        : IEntityTypeConfiguration<ApplicationBodyParameters>
    {
        public void Configure(EntityTypeBuilder<ApplicationBodyParameters> builder)
        {
            builder.HasKey(app => app.Id);
            builder.HasIndex(app => app.Id).IsUnique();
        }
    }
}
