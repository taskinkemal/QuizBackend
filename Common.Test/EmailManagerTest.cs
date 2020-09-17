using System.Linq;
using System.Net.Mail;
using Common.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Test
{
    [TestClass]
    public class EmailManagerTest
    {
        [TestMethod]
        public void SmtpClientHasCorrectProperties()
        {
            const string host = "myhost";
            const string user = "myuser";
            const string password = "password123";

            var smtpClient = EmailManager.CreateSmtpClient(host, user, password);

            Assert.AreEqual(host, smtpClient.Host);
            Assert.IsInstanceOfType(smtpClient.Credentials, typeof(System.Net.NetworkCredential));
            Assert.AreEqual(user, (smtpClient.Credentials as System.Net.NetworkCredential).UserName);
            Assert.AreEqual(password, (smtpClient.Credentials as System.Net.NetworkCredential).Password);
            Assert.AreEqual(EmailManager.Port, smtpClient.Port);
            Assert.AreEqual(SmtpDeliveryMethod.Network, smtpClient.DeliveryMethod);
        }

        [TestMethod]
        public void MailMessageHasCorrectProperties()
        {
            const string from = "from@mydomain.com";
            const string email = "myemail@mydomain.com";
            const string subject = "Test Email";
            const string body = "This is <b>my</b> email";

            var mailMessage = EmailManager.CreateMailMessage(from, email, subject, body);

            Assert.AreEqual(from, mailMessage.From.Address);
            Assert.AreEqual(subject, mailMessage.Subject);
            Assert.AreEqual(body, mailMessage.Body);
            Assert.AreEqual(1, mailMessage.To.Count);
            Assert.AreEqual(email, mailMessage.To.FirstOrDefault()?.Address);
            Assert.IsTrue(mailMessage.IsBodyHtml);
        }
    }
}
