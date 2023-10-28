using System.Threading;
using System.Threading.Tasks;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.CaloriAccount;
using Microsoft.EntityFrameworkCore;

namespace Calori.Application.Interfaces
{
    public interface ICaloriDbContext
    {
        DbSet<CaloriApplication> CaloriApplications { get; set; }
        DbSet<ApplicationBodyParameters> ApplicationBodyParameters { get; set; }
        DbSet<ApplicationAllergy> ApplicationAllergies { get; set; }
        DbSet<PersonalSlimmingPlan> PersonalSlimmingPlan { get; set; }
        DbSet<CaloriSlimmingPlan> CaloriSlimmingPlan { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}