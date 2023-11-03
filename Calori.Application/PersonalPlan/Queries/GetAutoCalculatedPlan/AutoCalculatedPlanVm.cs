using System;
using AutoMapper;
using Calori.Application.Common.Mappings;
using Calori.Domain.Models.ApplicationModels;
using Calori.Domain.Models.CaloriAccount;

namespace Calori.Application.PersonalPlan.Queries.GetAutoCalculatedPlan
{
    public class AutoCalculatedPlanVm : IMapWith<PersonalSlimmingPlan>
    {
        public int PersonalPlanId { get; set; }
        
        public double WeightLost { get; set; }
        public double HoursSaved { get; set; }
        public int DaysToChangePlan { get; set; }
        public int DaysLeft { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        
        public CaloriSlimmingPlan CurrentCaloriPlan { get; set; }
        public bool IsPaid { get; set; } = false;
        
        public string Message { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<PersonalSlimmingPlan, PersonalPlanDetailsVm>();
        }
    }
}