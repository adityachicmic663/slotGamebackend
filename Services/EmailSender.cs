using Microsoft.Extensions.Options;
using SlotGameBackend.Models;
using System.Net.Mail;
using System.Net;

namespace SlotGameBackend.Services
{
    public class EmailSender:IEmailSender
    {

        private readonly SmtpSettings _smtpSettings;

        public EmailSender(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }
        public  Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email address cannot be null or empty.", nameof(email));
            }

            var client = new SmtpClient(_smtpSettings.SmtpHost, _smtpSettings.SmtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.SmtpUsername, _smtpSettings.SmtpPassword),
                EnableSsl = _smtpSettings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(email));

            return  client.SendMailAsync(mailMessage);
        }
    }
}
