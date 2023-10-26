using System;
using System.Collections.Generic;
using Calori.Domain.Models.Enums;

namespace Calori.Domain.Models.ApplicationModels
{
    public class CaloriApplication
    {
        public int Id { get; set; }
    
        public CaloriGender? GenderId { get; set; }
    
        public decimal? Weight { get; set; }
    
        public int? Height { get; set; }
    
        public int? Goal { get; set; }
    
        public int? Age { get; set; }

        public string Email { get; set; } = string.Empty;
    
        public int? ApplicationBodyParametersId { get; set; }
    
        public ApplicationBodyParameters ApplicationBodyParameters { get; set; }
    
        //public int? ApplicationAllergyId { get; set; }
    
        public List<ApplicationAllergy> ApplicationAllergies { get; set; }
    
        public CaloriActivityLevel? ActivityLevelId { get; set; }
    
        public DateTime? CreatedAt { get; set; }
    
        public string AnotherAllergy { get; set; } = String.Empty;
        
        public int? DailyCalories { get; set; }
        
        public int? Ration { get; set; }
        
        public int? PersonalSlimmingPlanId { get; set; }
    }
}