using System;
using System.Linq;
using System.Net.Mail;
using Common.Implementations;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Common.Test
{
    [TestClass]
    public class EmailManagerTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            const string from = "testfrom";
            const string host = "testhost";
            const string user = "testuser";
            const string password = "testpassword";

            var settings = new AppSettings
            {
                Email = new EmailSettings
                {
                    From = from,
                    Host = host,
                    User = user,
                    Password = password
                }
            };

            var options = new Mock<IOptions<AppSettings>>();
            options.Setup(c => c.Value).Returns(settings);

            var sut = new EmailManager(options.Object);

            Assert.AreSame(settings.Email, sut.Settings);
        }

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

        [TestMethod]
        public void SmtpSendIsCalled()
        {
            const string from = "test.from@mail.com";
            const string host = "testhost";
            const string user = "testuser";
            const string password = "testpassword";

            var settings = new AppSettings
            {
                Email = new EmailSettings
                {
                    From = from,
                    Host = host,
                    User = user,
                    Password = password
                }
            };

            var options = new Mock<IOptions<AppSettings>>();
            options.SetupGet(c => c.Value).Returns(settings);

            var sut = new EmailManager(options.Object);

            Exception innerException = new Exception();

            try
            {
                sut.Send("test@test.com", "subject", "body");
            }
            catch (System.Net.Mail.SmtpException exc)
            {
                innerException = exc.InnerException;
            }

            Assert.IsTrue(innerException is System.Net.Sockets.SocketException);
            Assert.AreEqual("No such host is known.", innerException.Message);
        }
    }
}
