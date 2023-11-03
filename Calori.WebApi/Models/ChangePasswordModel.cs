using System.ComponentModel.DataAnnotations;
using Calori.Application.PersonalPlan.Queries.GetAutoCalculatedPlan;

namespace Calori.WebApi.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "The 'Current Password' field is required.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "The 'New Password' field is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; }
    }
}