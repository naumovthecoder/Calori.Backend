namespace Calori.Domain.Models.ApplicationModels
{
    public class ApplicationBodyParameters
    {
        public int Id { get; set; }
        public int? MinWeight { get; set; }
        public int? MaxWeight { get; set; }
        public decimal? BMI { get; set; }
        public decimal? BMR { get; set; }
    }
}
