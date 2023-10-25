using System.ComponentModel.DataAnnotations;

namespace Calori.Domain.Models.Enums
{
    public enum CaloriActivityLevel
    {
        // Малоактивная
        [Display(Name = "Inactive")]
        Inactive = 0,

        // Легкая
        [Display(Name = "Light")]
        Light = 1,
    
        // Умеренная
        [Display(Name = "Moderate")]
        Moderate = 2,
    }
}
