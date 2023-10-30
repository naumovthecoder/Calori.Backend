using System.ComponentModel.DataAnnotations;
using Calori.Domain.Models.Auth;
using MediatR;

namespace Calori.Application.Auth.Commands.Login
{
    public class LoginUserCommand: IRequest<LoginResponse>
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}