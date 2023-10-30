using AutoMapper;
using Calori.Application.Auth.Commands.ResetPassword;
using Calori.Application.Common.Mappings;

namespace Calori.WebApi.Models
{
    public class ResetPassModel : IMapWith<ResetPasswordCommand>
    {
        public string Email { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<ResetPassModel, ResetPasswordCommand>();
        }
    }
}