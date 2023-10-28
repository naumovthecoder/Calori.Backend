using Calori.Application.Interfaces;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.CaloriAccount;
using Calori.Persistence.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Calori.Persistence
{
    public class CaloriDbContext : DbContext, ICaloriDbContext
    {
        public DbSet<CaloriApplication> CaloriApplications { get; set; }
        public DbSet<ApplicationBodyParameters> ApplicationBodyParameters { get; set; }
        public DbSet<ApplicationAllergy> ApplicationAllergies { get; set; }
        public DbSet<PersonalSlimmingPlan> PersonalSlimmingPlan { get; set; }
        public DbSet<CaloriSlimmingPlan> CaloriSlimmingPlan { get; set; }
        public CaloriDbContext(DbContextOptions<CaloriDbContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ApplicationConfiguration());
            builder.ApplyConfiguration(new ApplicationBodyParametersConfiguration());
            builder.ApplyConfiguration(new ApplicationAllergyConfiguration());
            builder.ApplyConfiguration(new CaloriSlimmingPlanConfiguration());
            builder.ApplyConfiguration(new PersonalSlimmingPlanConfiguration());
            base.OnModelCreating(builder);
        }
    }
}