using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Test
{
    [TestClass]
    public class AppSettingsTest
    {
        [TestMethod]
        public void AppSettingsConstructor()
        {
            const string dbConnectionString = "connectionstring";
            const string apiKey = "apiKey";
            const string from = "from";
            const string host = "host";
            const string user = "user";
            const string password = "password";
            var emailSettings = new EmailSettings
            {
                Host = host,
                From = from,
                User = user,
                Password = password
            };

            var actual = new AppSettings
            {
                ConnectionString = dbConnectionString,
                ApiKey = apiKey,
                Email = emailSettings
            };

            Assert.AreEqual(dbConnectionString, actual.ConnectionString);
            Assert.AreEqual(apiKey, actual.ApiKey);
            Assert.AreSame(emailSettings, actual.Email);
            Assert.AreEqual(host, actual.Email.Host);
            Assert.AreEqual(from, actual.Email.From);
            Assert.AreEqual(user, actual.Email.User);
            Assert.AreEqual(password, actual.Email.Password);
        }
    }
}
