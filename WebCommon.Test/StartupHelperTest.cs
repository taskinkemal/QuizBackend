using System;
using System.Linq;
using Common;
using Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace WebCommon.Test
{
    [TestClass]
    public class StartupHelperTest
    {
        [TestMethod]
        public void InjectDependenciesCoversAllDependencies()
        {
            var containsAll = true;
            var collection = new ServiceCollection();

            StartupHelper.InjectDependencies(collection);
            using (var provider = collection.BuildServiceProvider())
            {
                var type = typeof(IDependency);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p) && !type.IsInterface);

                foreach (var t in types)
                {
                    var implementation = provider.GetService(t);

                    if (implementation == null)
                    {
                        containsAll = false;
                        break;
                    }
                }
            }

            Assert.IsTrue(containsAll);
        }

        [TestMethod]
        public void ConfigureSettings()
        {
            var collection = new ServiceCollection();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c.GetSection("AppSettings")).Returns(Mock.Of<IConfigurationSection>());
            StartupHelper.ConfigureAppSettings(collection, configuration.Object);

            using (var provider = collection.BuildServiceProvider())
            {
                Assert.IsNotNull(provider.GetService<IConfigureOptions<AppSettings>>());
            }
        }

        [TestMethod]
        public void SetNewtonsoftSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            StartupHelper.SetNewtonsoftSerializerSettings(settings);

            Assert.IsNotNull(settings.Converters.OfType<Newtonsoft.Json.Converters.StringEnumConverter>());
            Assert.AreEqual(NullValueHandling.Ignore, settings.NullValueHandling);
        }
    }
}
