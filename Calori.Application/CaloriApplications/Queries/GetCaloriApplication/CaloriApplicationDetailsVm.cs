using System;
using System.Collections.Generic;
using AutoMapper;
using Calori.Application.Common.Mappings;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.Enums;

namespace Calori.Application.CaloriApplications.Queries.GetCaloriApplication
{
    public class CaloriApplicationDetailsVm : IMapWith<CaloriApplication>
    {
        public int Id { get; set; }
        public CaloriGender? GenderId { get; set; }
        public decimal? Weight { get; set; }
        public int? Height { get; set; }
        public int? Goal { get; set; }
        public int? Age { get; set; }
        public string? Email { get; set; }
        public int? ApplicationBodyParametersId { get; set; }
        public ApplicationBodyParameters? ApplicationBodyParameters { get; set; }
        public int? ApplicationAllergyId { get; set; }
        public List<ApplicationAllergy>? ApplicationAllergies { get; set; }
        public CaloriActivityLevel? ActivityLevelId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? AnotherAllergy { get; set; }
        

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CaloriApplication, CaloriApplicationDetailsVm>()
                .ForMember(noteVm => noteVm.GenderId,
                    opt =>
                        opt.MapFrom(note => note.GenderId))

                .ForMember(noteVm => noteVm.Weight,
                    opt =>
                        opt.MapFrom(note => note.Weight))

                .ForMember(noteVm => noteVm.Id,
                    opt =>
                        opt.MapFrom(note => note.Id))

                .ForMember(noteVm => noteVm.Height,
                    opt =>
                        opt.MapFrom(note => note.Height))

                .ForMember(noteVm => noteVm.Goal,
                    opt =>
                        opt.MapFrom(note => note.Goal))

                .ForMember(noteVm => noteVm.Age,
                    opt =>
                        opt.MapFrom(note => note.Age))

                .ForMember(noteVm => noteVm.Email,
                    opt =>
                        opt.MapFrom(note => note.Email))

                .ForMember(noteVm => noteVm.ApplicationBodyParametersId,
                    opt =>
                        opt.MapFrom(note => note.ApplicationBodyParametersId))

                .ForMember(noteVm => noteVm.ApplicationBodyParameters,
                    opt =>
                        opt.MapFrom(note => note.ApplicationBodyParameters))

                .ForMember(noteVm => noteVm.ApplicationAllergyId,
                    opt =>
                        opt.MapFrom(note => note.ApplicationAllergyId))

                .ForMember(noteVm => noteVm.ApplicationAllergies,
                    opt =>
                        opt.MapFrom(note => note.ApplicationAllergies))

                .ForMember(noteVm => noteVm.ActivityLevelId,
                    opt =>
                        opt.MapFrom(note => note.ActivityLevelId))

                .ForMember(noteVm => noteVm.CreatedAt,
                    opt =>
                        opt.MapFrom(note => note.CreatedAt))

                .ForMember(noteVm => noteVm.AnotherAllergy,
                    opt =>
                        opt.MapFrom(note => note.AnotherAllergy));
        }
    }
}