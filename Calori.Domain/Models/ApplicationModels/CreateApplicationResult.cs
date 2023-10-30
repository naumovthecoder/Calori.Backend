namespace Calori.Domain.Models.ApplicationModels
{
    public class CreateApplicationResult
    {
        public CaloriApplication Application { get; set; }
        public string Token { get; set; }
    }
}