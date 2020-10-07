using Common.Implementations;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace WebApplication.Test
{
    [TestClass]
    public class StartupTest
    {
        [TestMethod]
        public void ConfigureServicesRegistersDependenciesCorrectly()
        {
            //  Arrange

            //  Setting up the stuff required for Configuration.GetConnectionString("DefaultConnection")
            var configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["DefaultConnection"]).Returns("TestConnectionString");
            var configurationStub = new Mock<IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).Returns(configurationSectionStub.Object);
            configurationStub.Setup(c => c.GetSection("AppSettings")).Returns(Mock.Of<IConfigurationSection>());

            var services = new ServiceCollection();
            var target = new Startup(configurationStub.Object);

            //  Act

            target.ConfigureServices(services);
            //  Mimic internal asp.net core logic.
            services.AddTransient<TestController>();
            services.AddSingleton<ILogManager, LogManager>();

            //  Assert

            var serviceProvider = services.BuildServiceProvider();

            var controller = serviceProvider.GetService<TestController>();
            Assert.IsNotNull(controller);
        }
    }

    public class TestController : Controller
    {
    }
}
