using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Common.Implementations;
using Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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

        [TestMethod]
        public async Task Swagger01()
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
            services.AddSingleton<IWebHostEnvironment, DummyWebHostEnvironment>();

            var listener = new DummyDiagnosticListener("");
            services.AddSingleton<DiagnosticSource>(listener);
            //  Assert

            var serviceProvider = services.BuildServiceProvider();

            var builder = new ApplicationBuilder(serviceProvider);
            target.ConfigureInternal(builder, true, b => 0);

            var requestDelegate = builder.Build();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/v1/swagger.json";
            httpContext.Request.Method = "GET";

            await requestDelegate.Invoke(httpContext);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task Swagger02()
        {
            HttpStatusCode result;

            var webHostBuilder = new WebHostBuilder()
            .UseEnvironment("Test")
            .UseStartup<Startup>();

            using (var server = new TestServer(webHostBuilder))
            {
                using (var client = server.CreateClient())
                {
                    var response = await client.GetAsync("/swagger/v1/swagger.json");
                    result = response.StatusCode;
                }
            }

            Assert.AreEqual(HttpStatusCode.OK, result);
        }
    }

    public class DummyDiagnosticListener : DiagnosticListener
    {
        public new string Name { get; }

        public DummyDiagnosticListener(string name) : base(name) { this.Name = name; }
    }

    public class DummyWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get => throw new Exception("exc 11"); set => throw new Exception("exc 1"); }
        public IFileProvider WebRootFileProvider { get => Mock.Of<IFileProvider>(); set => throw new Exception("exc 2"); }
        public string ApplicationName { get => throw new Exception("exc 33"); set => throw new Exception("exc 3"); }
        public IFileProvider ContentRootFileProvider { get => Mock.Of<IFileProvider>(); set => throw new Exception("exc 4"); }
        public string ContentRootPath { get => throw new Exception("exc 55"); set => throw new Exception("exc 5"); }
        public string EnvironmentName { get => "Release"; set => throw new Exception("exc 6"); }
    }

    public class TestController : Controller
    {
    }
}
