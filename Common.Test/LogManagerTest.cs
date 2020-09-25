using System;
using Common.Implementations;
using Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog.Events;

namespace Common.Test
{
    [TestClass]
    public class LogManagerTest
    {
        [TestMethod]
        public void AddLogMessage()
        {
            const LogCategory category = LogCategory.Email;
            const string message = "log message";
            const LogEventLevel level = LogEventLevel.Debug;

            var actualMessage = "";
            var actualLevel = LogEventLevel.Fatal;
            Exception actualException = null;
            object[] actualArgs = null;
            var actualCategory = LogCategory.GeneralError;

            var adapter = new Mock<ILogAdapter>();
            adapter
                .Setup(c => c.Write(It.IsAny<LogEventLevel>(), It.IsAny<Exception>(), It.IsAny<string>(),
                    It.IsAny<object[]>()))
                .Callback((LogEventLevel l, Exception e, string m, object[] args) =>
                {
                    actualLevel = l;
                    actualMessage = m;
                    actualException = e;
                    actualArgs = args;
                    actualCategory = (LogCategory)(args[args.Length - 1] as LogParameters).CategoryId;
                });

            var sut = new LogManager(adapter.Object);

            sut.AddLog(category, message, level);

            Assert.AreEqual(message + " {@LogParameters}", actualMessage);
            Assert.AreEqual(level, actualLevel);
            Assert.AreEqual(category, actualCategory);
            Assert.IsNull(actualException);
            Assert.AreEqual(1, actualArgs.Length);
        }

        [TestMethod]
        public void AddLogMessageWithMessageNull()
        {
            const LogCategory category = LogCategory.Email;
            const string message = null;
            const LogEventLevel level = LogEventLevel.Debug;

            var actualMessage = "";
            var actualLevel = LogEventLevel.Fatal;
            Exception actualException = null;
            object[] actualArgs = null;
            var actualCategory = LogCategory.GeneralError;

            var adapter = new Mock<ILogAdapter>();
            adapter
                .Setup(c => c.Write(It.IsAny<LogEventLevel>(), It.IsAny<Exception>(), It.IsAny<string>(),
                    It.IsAny<object[]>()))
                .Callback((LogEventLevel l, Exception e, string m, object[] args) =>
                {
                    actualLevel = l;
                    actualMessage = m;
                    actualException = e;
                    actualArgs = args;
                    actualCategory = (LogCategory)(args[args.Length - 1] as LogParameters).CategoryId;
                });

            var sut = new LogManager(adapter.Object);

            sut.AddLog(category, message, level);

            Assert.AreEqual(" {@LogParameters}", actualMessage);
            Assert.AreEqual(level, actualLevel);
            Assert.AreEqual(category, actualCategory);
            Assert.IsNull(actualException);
            Assert.AreEqual(1, actualArgs.Length);
        }

        [TestMethod]
        public void AddLogMessageWithArgs()
        {
            const LogCategory category = LogCategory.Email;
            const string message = "log message {myArg}";
            const LogEventLevel level = LogEventLevel.Debug;
            var myArg = 43;
            object[] args = { myArg };

            var actualMessage = "";
            var actualLevel = LogEventLevel.Fatal;
            Exception actualException = null;
            object[] actualArgs = null;
            var actualCategory = LogCategory.GeneralError;

            var adapter = new Mock<ILogAdapter>();
            adapter
                .Setup(c => c.Write(It.IsAny<LogEventLevel>(), It.IsAny<Exception>(), It.IsAny<string>(),
                    It.IsAny<object[]>()))
                .Callback((LogEventLevel l, Exception e, string m, object[] args) =>
                {
                    actualLevel = l;
                    actualMessage = m;
                    actualException = e;
                    actualArgs = args;
                    actualCategory = (LogCategory)(args[args.Length - 1] as LogParameters).CategoryId;
                });

            var sut = new LogManager(adapter.Object);

            sut.AddLog(category, message, level, args);

            Assert.AreEqual(message + " {@LogParameters}", actualMessage);
            Assert.AreEqual(level, actualLevel);
            Assert.AreEqual(category, actualCategory);
            Assert.IsNull(actualException);
            Assert.AreEqual(2, actualArgs.Length);
            Assert.AreEqual(myArg, actualArgs[0]);
        }

        [TestMethod]
        public void AddExceptionLog()
        {
            const LogCategory category = LogCategory.Email;
            var exception = new Exception("my exception");

            var actualMessage = "";
            var actualLevel = LogEventLevel.Fatal;
            Exception actualException = null;
            object[] actualArgs = null;
            var actualCategory = LogCategory.GeneralError;

            var adapter = new Mock<ILogAdapter>();
            adapter
                .Setup(c => c.Write(It.IsAny<LogEventLevel>(), It.IsAny<Exception>(), It.IsAny<string>(),
                    It.IsAny<object[]>()))
                .Callback((LogEventLevel l, Exception e, string m, object[] args) =>
                {
                    actualLevel = l;
                    actualMessage = m;
                    actualException = e;
                    actualArgs = args;
                    actualCategory = (LogCategory)(args[args.Length - 1] as LogParameters).CategoryId;
                });

            var sut = new LogManager(adapter.Object);

            sut.AddLog(category, exception);

            Assert.AreEqual("Quiz exception {@LogParameters}", actualMessage);
            Assert.AreEqual(LogEventLevel.Error, actualLevel);
            Assert.AreEqual(category, actualCategory);
            Assert.AreSame(exception, actualException);
            Assert.AreEqual(1, actualArgs.Length);
        }
    }
}
