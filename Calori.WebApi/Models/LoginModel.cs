using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Calori.Application.Auth.Commands.Login;
using Calori.Application.Common.Mappings;

namespace Calori.WebApi.Models
{
    public class LoginModel : IMapWith<LoginUserCommand>
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<LoginModel, LoginUserCommand>();
        }
    }
}