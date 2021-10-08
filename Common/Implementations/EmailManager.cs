using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using Common.Interfaces;
using Microsoft.Extensions.Options;

[assembly: InternalsVisibleTo("Common.Test")]
namespace Common.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailManager : IEmailManager
    {
        internal EmailSettings Settings { get; }
        private readonly SmtpClient smtpClient;
        internal const int Port = 25;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public EmailManager(IOptions<AppSettings> settings)
        {
            Settings = settings.Value.Email;
            smtpClient = CreateSmtpClient(Settings.Host, Settings.User, Settings.Password);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        [ExcludeFromCodeCoverage]
        public void Send(string email, string subject, string body)
        {
            var message = CreateMailMessage(Settings.From, email, subject, body);
            smtpClient.Send(message);
        }

        internal static MailMessage CreateMailMessage(string from, string email, string subject, string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                IsBodyHtml = true,
                Body = body
            };

            message.To.Add(new MailAddress(email));

            return message;
        }

        internal static SmtpClient CreateSmtpClient(string host, string user, string password)
        {
            return new SmtpClient(host, Port)
            {
                Credentials = new System.Net.NetworkCredential(user, password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
        }
    }
}
