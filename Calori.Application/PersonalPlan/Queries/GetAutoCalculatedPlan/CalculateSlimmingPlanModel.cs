using Calori.Domain.Models.Enums;

namespace Calori.Application.PersonalPlan.Queries.GetAutoCalculatedPlan
{
    public class CalculateSlimmingPlanModel
    {
        public int CurrentWeek { get; set; }
        public decimal Weight { get; set; }
        public int CaloriNeeds { get; set; }
        public int Gender { get; set; }
        public int Goal { get; set; }
        public CaloriActivityLevel CaloriActivityLevel { get; set; }
    }
}