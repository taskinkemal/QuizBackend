using System;
using Common.Implementations;
using Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace WebApplication.Test
{
    [TestClass]
    public class StartupTest
    {
        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ConfigureServicesRegistersDependenciesCorrectly(bool isDevelopment)
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
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            //  Assert

            var serviceProvider = services.BuildServiceProvider();

            var builder = new ApplicationBuilder(serviceProvider);
            target.ConfigureInternal(builder, isDevelopment, b => 0);

            var controller = serviceProvider.GetService<TestController>();
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        public void CreateWebHostBuilder()
        {
            var configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["DefaultConnection"]).Returns("TestConnectionString");
            var configurationStub = new Mock<IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).Returns(configurationSectionStub.Object);
            configurationStub.Setup(c => c.GetSection("AppSettings")).Returns(Mock.Of<IConfigurationSection>());

            var services = new ServiceCollection();
            var target = new Startup(configurationStub.Object);

            var builder = Program.CreateWebHostBuilder(new string[] { });

            Assert.IsNotNull(builder);
        }
    }

    public class TestController : Controller
    {
    }
}
