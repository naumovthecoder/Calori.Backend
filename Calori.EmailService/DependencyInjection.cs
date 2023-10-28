using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Calori.Application.Interfaces;
using Calori.EmailService;

namespace Calori.EmailService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEmailService(
            this IServiceCollection services, IConfiguration configuration)
        {
            string smtpServer = configuration["SmtpServer"];
            int smtpPort = int.Parse(configuration["SmtpPort"]);
            string smtpUsername = configuration["SmtpUsername"];
            string smtpPassword = configuration["SmtpPassword"];
            
            services.AddSingleton<IEmailService>(new EmailService(smtpServer, smtpPort, smtpUsername, smtpPassword));

            return services;
        }
    }
}