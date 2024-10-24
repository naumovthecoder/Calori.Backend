namespace Calori.Domain.Models.ApplicationModels
{
    public class RecommendedIdealWeightDto
    {
        public int? MinWeight { get; set; }
        public int? MaxWeight { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
