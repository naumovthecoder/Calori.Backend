using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Calori.Application.Auth.Commands.Login;
using Calori.Application.Auth.Commands.Register;
using Calori.Application.Common.Mappings;

namespace Calori.Application.Auth
{
    public class RegisterModel : IMapWith<RegisterUserCommand>
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<RegisterModel, RegisterUserCommand>();
        }
    }
}