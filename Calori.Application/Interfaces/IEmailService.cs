using System.Threading.Tasks;

namespace Calori.Application.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body);
    }
}