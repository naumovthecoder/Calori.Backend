using System.ComponentModel.DataAnnotations;

namespace Calori.Domain.Models.Enums
{
    public enum Rations
    {
        [Display(Name = "None")]
        None = 0,
    
        [Display(Name = "Other")]
        Other = 1,
    
        [Display(Name = "Lactose")]
        Lactose = 2,
    }
}