using Calori.Application.Interfaces;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.CaloriAccount;
using Calori.Persistence.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Calori.WebApi
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
            
            builder.Entity<CaloriSlimmingPlan>().HasData(
                new CaloriSlimmingPlan { Id = 1, Calories = 1250 },
                new CaloriSlimmingPlan { Id = 2, Calories = 1500 },
                new CaloriSlimmingPlan { Id = 3, Calories = 1750 },
                new CaloriSlimmingPlan { Id = 4, Calories = 2000 },
                new CaloriSlimmingPlan { Id = 5, Calories = 2250 },
                new CaloriSlimmingPlan { Id = 6, Calories = 2500 }
            );
            
            base.OnModelCreating(builder);
        }
    }
}