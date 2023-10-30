using System.ComponentModel.DataAnnotations;
using Calori.Domain.Models.Auth;
using MediatR;

namespace Calori.Application.Auth.Commands.Register
{
    public class RegisterUserCommand : IRequest<RegisterResponse>
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}