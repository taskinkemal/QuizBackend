using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Moq;
using WebApplication.Controllers;

namespace WebApplication.Test
{
    [TestClass]
    public class OptionsTest
    {
        [TestMethod]
        public async Task Get()
        {
            const int optionId = 56;
            var options = new List<Option>
            {
                new Option(),
                new Option()
            };

            var optionsManager = new Mock<IOptionManager>();

            optionsManager.Setup(c => c.GetQuizOptions(optionId))
                .Returns(Task.FromResult(options));

            var sut = new OptionsController(optionsManager.Object);

            var result = await sut.Get(optionId);

            Assert.AreEqual(options.Count, result.ToList().Count);
        }
    }
}
