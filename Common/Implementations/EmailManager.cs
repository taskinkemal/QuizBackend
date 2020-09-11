using System.Net.Mail;
using Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Common.Implementations
{
    public class EmailManager : IEmailManager
    {
        private readonly EmailSettings settings;

        public EmailManager(IOptions<AppSettings> settings)
        {
            this.settings = settings.Value.Email;
        }

        public void Send(string email, string subject, string body)
        {
            var message = new MailMessage();
            var smtpClient = new SmtpClient(settings.Host, 25);
            message.From = new MailAddress(settings.From);
            message.To.Add(new MailAddress(email));
            message.Subject = subject;
            message.IsBodyHtml = true; //to make message body as html  
            message.Body = body;
            //smtp.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential(settings.User, settings.Password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Send(message);
        }
    }
}
