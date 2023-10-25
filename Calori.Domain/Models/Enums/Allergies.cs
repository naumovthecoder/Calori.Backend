using System.ComponentModel.DataAnnotations;

namespace Calori.Domain.Models.Enums
{
    public enum Allergies
    {
        [Display(Name = "None")]
        None = 0,
    
        [Display(Name = "Other")]
        Other = 1,
    
        [Display(Name = "Lactose")]
        Lactose = 2,

        [Display(Name = "Gluten")]
        Gluten = 3,
    
        [Display(Name = "Nuts")]
        Nuts = 4,

        [Display(Name = "Fish")]
        Fish = 5,
    
        [Display(Name = "Citrus")]
        Citrus = 6
    }
}
