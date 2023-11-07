using System.ComponentModel.DataAnnotations;

namespace Calori.Domain.Models.Enums
{
    public enum SubscriptionStatus
    {
        [Display(Name = "Canceled")]
        Canceled = 0,
    
        [Display(Name = "Active")]
        Active = 1,
    
        [Display(Name = "Paused")]
        Paused = 2,
        
        [Display(Name = "Not Started")]
        NotStarted = 3,
        
        [Display(Name = "Resumed")]
        Resumed = 4,
    }
}