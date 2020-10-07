using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Implementations;
using Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Moq;

namespace BusinessLayer.Test
{
    [TestClass]
    public class OptionManagerTest
    {
        [TestMethod]
        public async Task GetQuizOptions()
        {
            List<Option> options;
            const int questionCount = 2;
            const int optionCount = 4;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new OptionManager(context, logManager);

                var testData = await ManagerTestHelper.CreateQuizAsync(context, questionCount, optionCount);

                options = await sut.GetQuizOptions(testData.QuizId);
            }

            Assert.IsNotNull(options);
            Assert.AreEqual(questionCount * optionCount, options.Count);
        }

        [TestMethod]
        public async Task GetQuizOptionsNoRows()
        {
            List<Option> options;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new OptionManager(context, logManager);

                var testData = await ManagerTestHelper.CreateQuizAsync(context, 0, 0);

                options = await sut.GetQuizOptions(testData.QuizId);
            }

            Assert.IsNotNull(options);
            Assert.AreEqual(0, options.Count);
        }

        [TestMethod]
        public async Task InsertOptionNull()
        {
            int result;
            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new OptionManager(context, logManager);
                result = await sut.InsertOptionInternalAsync(new Option());
            }

            Assert.AreEqual(1, result);
        }
    }
}
