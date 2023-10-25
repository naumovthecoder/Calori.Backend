using Calori.Domain.Models.Enums;

namespace Calori.Domain.Models.ApplicationModels
{
    public class ApplicationAllergy
    {
        public int Id { get; set; }

        public int? ApplicationId { get; set; }
        
        //public CaloriApplication Application { get; set; }

        public Allergies? Allergy { get; set; }
    }
}
