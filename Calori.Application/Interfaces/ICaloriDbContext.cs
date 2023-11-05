using System.Threading;
using System.Threading.Tasks;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.CaloriAccount;
using Calori.Domain.Models.Payment;
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
        DbSet<SubscriptionDetails> SubscriptionDetails { get; set; }
        DbSet<PaymentPeriod> PaymentPeriods { get; set; }
        DbSet<UserPayment> UserPayments { get; set; }
        DbSet<CaloriShippingData> CaloriShippingData { get; set; }
        DbSet<CaloriFeedback> CaloriFeedback { get; set; }
        DbSet<CaloriPrice> CaloriPrices { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}