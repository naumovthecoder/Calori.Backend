// using System;
// using System.Net;
// using System.Net.Mail;
// using System.Threading.Tasks;
// using Calori.Application.Interfaces;
// using Microsoft.Extensions.Configuration;
//
// namespace Calori.EmailService
// {
//     public class EmailService : IEmailService
//     {
//         private readonly string smtpServer;
//         private readonly int smtpPort;
//         private readonly string smtpUsername;
//         private readonly string smtpPassword;
//         private readonly IConfiguration _configuration;
//
//         public EmailService(IConfiguration configuration)
//         {
//             _configuration = configuration;
//             
//             smtpServer = _configuration["EmailSettings:SmtpServer"];
//             smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
//             smtpUsername = _configuration["EmailSettings:SmtpUsername"];
//             smtpPassword = _configuration["EmailSettings:SmtpPassword"];
//         }
//
//         public async Task SendEmailAsync(string toEmail, string subject, string body)
//         {
//             bool enableSSL = true;
//
//             SmtpClient client = new SmtpClient(smtpServer)
//             {
//                 Port = smtpPort,
//                 Credentials = new NetworkCredential(smtpUsername, smtpPassword),
//                 EnableSsl = enableSSL,
//             };
//
//             MailMessage message = new MailMessage
//             {
//                 From = new MailAddress(smtpUsername),
//                 Subject = subject,
//                 Body = body,
//             };
//
//             message.To.Add(toEmail);
//
//             try
//             {
//                 await client.SendMailAsync(message);
//                 Console.WriteLine("Письмо успешно отправлено.");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Ошибка отправки письма: " + ex.Message);
//             }
//         }
//     }
//
//     public class EmailResponseException : Exception
//     {
//         private readonly string _message;
//         public EmailResponseException(string message)
//         {
//             _message = message;
//         }
//     }
// }
