using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace Calori.EmailRabbotMQReceiver
{

    class EmailReceiver
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 5672
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "email_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var emailContent = Encoding.UTF8.GetString(body);

                    try
                    {
                        var email = JsonSerializer.Deserialize<EmailContent>(emailContent);
                        SendEmail(email);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        Console.WriteLine("Письмо успешно отправлено.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка при отправке письма: " + ex.Message);
                        // Можно реализовать механизм повторной отправки или логгирования ошибок
                    }
                };

                channel.BasicConsume(queue: "email_queue",
                    autoAck: false,
                    consumer: consumer);

                Console.WriteLine("Waiting for email messages. To exit, press Ctrl+C.");
                Console.ReadLine();
            }
        }

        private static void SendEmail(EmailContent email)
        {
            var smtpServer = "smtp.mail.me.com";
            var smtpPort = 587;
            var smtpUsername = "naumovthecoder@icloud.com";
            var smtpPassword = "celp-wzqn-snkm-ubfw";

            SmtpClient client = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,
            };

            MailMessage message = new MailMessage
            {
                From = new MailAddress(smtpUsername),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = false, 
            };

            message.To.Add(email.ToEmail);

            client.Send(message);
        }
    }

    public class EmailContent
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}