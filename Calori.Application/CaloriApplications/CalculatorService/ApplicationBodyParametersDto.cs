namespace Calori.Application.CaloriApplications.Commands.CreateApplication.CalculatorService
{
    public class ApplicationBodyParametersDto
    {
        public int? MinWeight { get; set; }
        public int? MaxWeight { get; set; }
        public decimal? BMI { get; set; }
        public decimal? BMR { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}