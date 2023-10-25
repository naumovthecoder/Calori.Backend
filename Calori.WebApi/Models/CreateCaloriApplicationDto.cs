using System.Collections.Generic;
using AutoMapper;
using Calori.Application.CaloriApplications.Commands.CreateApplication;
using Calori.Application.Common.Mappings;
using Calori.Domain.Models.Enums;

namespace Calori.WebApi.Models
{
    public class CreateCaloriApplicationDto : IMapWith<CreateCaloriApplicationCommand>
    {
        public int? Gender { get; set; }
        public decimal? Weight { get; set; }
        public int? Height { get; set; }
        public int? Age { get; set; }
        public int? Activity { get; set; }
        public int? Goal { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<int>? Allergies { get; set; }
        public string? AnotherAllergy { get; set; } = string.Empty;
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateCaloriApplicationDto, CreateCaloriApplicationCommand>()
                .ForMember(noteCommand => noteCommand.Height,
                    opt => 
                        opt.MapFrom(noteDto => noteDto.Height))
            
                .ForMember(noteCommand => noteCommand.Age,
                    opt => 
                        opt.MapFrom(noteDto => noteDto.Age))
            
                .ForMember(noteCommand => noteCommand.Activity,
                    opt => 
                        opt.MapFrom(noteDto => noteDto.Activity))
            
                .ForMember(noteCommand => noteCommand.Goal,
                    opt => 
                        opt.MapFrom(noteDto => noteDto.Goal))
            
                .ForMember(noteCommand => noteCommand.Email,
                    opt => 
                        opt.MapFrom(noteDto => noteDto.Email))
            
                .ForMember(noteCommand => noteCommand.Allergies,
                    opt => 
                        opt.MapFrom(noteDto => noteDto.Allergies))
            
                .ForMember(noteCommand => noteCommand.AnotherAllergy,
                    opt => 
                        opt.MapFrom(noteDto => noteDto.AnotherAllergy))
            
                .ForMember(noteCommand => noteCommand.Gender,
                    opt => 
                        opt.MapFrom(noteDto => noteDto.Gender))
            
                .ForMember(noteCommand => noteCommand.Weight,
                    opt => 
                        opt.MapFrom(noteDto => noteDto.Weight));
        }
    }
}