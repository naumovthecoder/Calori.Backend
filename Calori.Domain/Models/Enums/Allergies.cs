using System.ComponentModel.DataAnnotations;

namespace Calori.Domain.Models.Enums
{
    public enum Allergies
    {
        [Display(Name = "Lactose")]
        Lactose = 1,

        [Display(Name = "Gluten")]
        Gluten = 2,
    
        [Display(Name = "Nuts")]
        Nuts = 3,

        [Display(Name = "Fish")]
        Fish = 4,
    
        [Display(Name = "Citrus")]
        Citrus = 5,
        
        [Display(Name = "Vegeterian")]
        Vegeterian = 6,
    
        [Display(Name = "Vegan")]
        Vegan = 7
    }
}
