using System.ComponentModel.DataAnnotations;

namespace Calori.Domain.Models.CaloriAccount
{
    public enum PaymentStatus
    {
        [Display(Name = "Successful")]
        Successful = 0,
    
        [Display(Name = "Declined")]
        Declined = 1,
    
        [Display(Name = "Pending")]
        Pending = 2,
    
        [Display(Name = "Cancelled")]
        Cancelled = 3,
    
        [Display(Name = "Refunded")]
        Refunded = 4,
    
        [Display(Name = "Error")]
        Error = 5,
    
        [Display(Name = "Processing")]
        Processing = 6,
    
        [Display(Name = "On Hold")]
        OnHold = 7,
    
        [Display(Name = "Not Processed")]
        NotProcessed = 8,
    
        [Display(Name = "Pending Confirmation")]
        PendingConfirmation = 9
    }
}