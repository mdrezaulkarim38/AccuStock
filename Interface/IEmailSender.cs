using System.Threading.Tasks;

namespace AccuStock.Interface;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message, string attachmentPath = null);
}