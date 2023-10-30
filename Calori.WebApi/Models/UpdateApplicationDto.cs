using System.Collections.Generic;
using AutoMapper;
using Calori.Application.CaloriApplications.Commands.UpdateApplication;
using Calori.Application.Common.Mappings;
using Calori.Domain.Models.Enums;

namespace Calori.WebApi.Models
{
    public class UpdateApplicationDto : IMapWith<UpdateApplicationCommand>
    {
        // public int Id { get; set; }
        public CaloriGender Gender { get; set; }
        public decimal Weight { get; set; }
        public int Height { get; set; }
        public int Age { get; set; }
        public CaloriActivityLevel Activity { get; set; }
        public int Goal { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<int> Allergies { get; set; }
        public string AnotherAllergy { get; set; } = string.Empty;
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateApplicationDto, UpdateApplicationCommand>();
        }
    }
}