using System.ComponentModel.DataAnnotations;

namespace Calori.Application.Auth
{
    public class RegisterModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}