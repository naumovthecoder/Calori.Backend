using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using System.Threading.Tasks;
using Calori.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Calori.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var emailContent = new EmailContent
                {
                    ToEmail = toEmail,
                    Subject = subject,
                    Body = body
                };

                var factory = new ConnectionFactory
                {
                    HostName = "127.0.0.1",
                    UserName = "guest",
                    Password = "guest",
                    Port = 5672
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "email_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var jsonContent = JsonSerializer.Serialize(emailContent);
                    var bodyBytes = Encoding.UTF8.GetBytes(jsonContent);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true; // Гарантировать сохранение сообщений на диск

                    channel.BasicPublish(exchange: "",
                                        routingKey: "email_queue",
                                        basicProperties: properties,
                                        body: bodyBytes);

                    Console.WriteLine("The email has been successfully added to the queue to be sent.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when adding an email to the queue: " + ex.Message);
            }
        }
    }

    public class EmailContent
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
