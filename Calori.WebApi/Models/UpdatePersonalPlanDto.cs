using System;
using AutoMapper;
using Calori.Application.Common.Mappings;
using Calori.Application.PersonalPlan.Commands.UpdatePersonalSlimmingPlan;
using Calori.Domain.Models.Enums;

namespace Calori.WebApi.Models
{
    public class UpdatePersonalPlanDto: IMapWith<UpdatePersonalSlimmingPlanCommand>
    {
        public DateTime? StartDate { get; set; }
        public int? WeekNumber { get; set; }
        public decimal? Weight { get; set; }
        public CaloriActivityLevel? CaloriActivityLevel { get; set; }
        public int? CaloricNeeds { get; set; }
        public int? Goal { get; set; }
        public int? CaloriSlimmingPlanId { get; set; }
        public int? CurrentWeekDeficit { get; set; }
        public int? BurnedThisWeek { get; set; }
        public int? TotalBurned { get; set; }
        public CaloriGender? Gender { get; set; }
        public int? CurrentCalori { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdatePersonalPlanDto, UpdatePersonalSlimmingPlanCommand>();
        }
    }
}