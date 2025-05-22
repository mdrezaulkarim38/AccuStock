using System.Net.Mail;
using System.Threading.Tasks;
using AccuStock.Interface;
using Microsoft.Extensions.Configuration;

namespace AccuStock.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message, string attachmentPath = null!)
        {
            try
            {
                // Retrieve SMTP settings from configuration
                var smtpHost = _configuration["Smtp:Host"];
                var smtpPort = int.Parse(_configuration["Smtp:Port"]!);
                var smtpUsername = _configuration["Smtp:Username"];
                var smtpPassword = _configuration["Smtp:Password"];
                var fromEmail = _configuration["Smtp:FromEmail"];

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail!),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                if (!string.IsNullOrEmpty(attachmentPath) && System.IO.File.Exists(attachmentPath))
                {
                    using var attachment = new Attachment(attachmentPath);
                    mailMessage.Attachments.Add(attachment);
                    await client.SendMailAsync(mailMessage);
                }
                else
                {
                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Log the error (you can inject ILogger<EmailSender> if needed)
                Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
                throw; // Re-throw to let Hangfire retry
            }
        }
    }
}