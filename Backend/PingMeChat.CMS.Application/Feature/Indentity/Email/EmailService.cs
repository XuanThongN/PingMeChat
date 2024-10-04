using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PingMeChat.CMS.Application.Feature.Indentity.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }


    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration)
        {
            _smtpClient = new SmtpClient(configuration["Email:Smtp:Host"], int.Parse(configuration["Email:Smtp:Port"]))
            {
                Credentials = new NetworkCredential(configuration["Email:Smtp:Username"], configuration["Email:Smtp:Password"]),
                EnableSsl = true
            };
            _fromEmail = configuration["Email:From"];
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var mailMessage = new MailMessage(_fromEmail, to, subject, body)
            {
                IsBodyHtml = true
            };
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }

}