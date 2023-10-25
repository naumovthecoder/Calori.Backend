using System.ComponentModel.DataAnnotations;

namespace Calori.Domain.Models.Enums
{
    public enum CaloriGender
    {
        [Display(Name = "Male")]
        Male = 0,

        [Display(Name = "Female")]
        Female = 1,
    }
}
