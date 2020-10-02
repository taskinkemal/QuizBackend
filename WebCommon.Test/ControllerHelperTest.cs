using System;
using System.Net;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.TransferObjects;
using WebCommon.BaseControllers;

namespace WebCommon.Test
{
    [TestClass]
    public class ControllerHelperTest
    {
        [TestMethod]
        public void CreateResponseSimpleTypeBoolean()
        {
            var result = ControllerHelper.CreateResponse(true);

            Assert.IsInstanceOfType(result.Value, typeof(GenericWrapper<bool>));
            Assert.IsTrue((result.Value as GenericWrapper<bool>).Value);
        }

        [TestMethod]
        public void CreateResponseSimpleTypeEnum()
        {
            var value = LogCategory.GeneralError;
            var result = ControllerHelper.CreateResponse(value);

            Assert.IsInstanceOfType(result.Value, typeof(GenericWrapper<LogCategory>));
            Assert.AreEqual(value, (result.Value as GenericWrapper<LogCategory>).Value);
        }

        [TestMethod]
        public void CreateResponseSimpleTypeString()
        {
            var value = "teststring";
            var result = ControllerHelper.CreateResponse(value);

            Assert.IsInstanceOfType(result.Value, typeof(GenericWrapper<string>));
            Assert.AreEqual(value, (result.Value as GenericWrapper<string>).Value);
        }

        [TestMethod]
        public void CreateResponseSimpleTypeDecimal()
        {
            var value = 1.2M;
            var result = ControllerHelper.CreateResponse(value);

            Assert.IsInstanceOfType(result.Value, typeof(GenericWrapper<decimal>));
            Assert.AreEqual(value, (result.Value as GenericWrapper<decimal>).Value);
        }

        [TestMethod]
        public void CreateResponseSimpleTypeDateTime()
        {
            var value = new DateTime(2020, 5, 12);
            var result = ControllerHelper.CreateResponse(value);

            Assert.IsInstanceOfType(result.Value, typeof(GenericWrapper<DateTime>));
            Assert.AreEqual(value, (result.Value as GenericWrapper<DateTime>).Value);
        }

        [TestMethod]
        public void CreateResponseSimpleTypeDateTimeOffset()
        {
            var value = new DateTimeOffset(2020, 5, 12, 9, 0, 0, new TimeSpan(1, 0, 0));
            var result = ControllerHelper.CreateResponse(value);

            Assert.IsInstanceOfType(result.Value, typeof(GenericWrapper<DateTimeOffset>));
            Assert.AreEqual(value, (result.Value as GenericWrapper<DateTimeOffset>).Value);
        }

        [TestMethod]
        public void CreateResponseSimpleTypeDateTimeSpan()
        {
            var value = new TimeSpan(1, 0, 0);
            var result = ControllerHelper.CreateResponse(value);

            Assert.IsInstanceOfType(result.Value, typeof(GenericWrapper<TimeSpan>));
            Assert.AreEqual(value, (result.Value as GenericWrapper<TimeSpan>).Value);
        }

        [TestMethod]
        public void CreateResponseSimpleTypeDateGuid()
        {
            var value = Guid.NewGuid();
            var result = ControllerHelper.CreateResponse(value);

            Assert.IsInstanceOfType(result.Value, typeof(GenericWrapper<Guid>));
            Assert.AreEqual(value, (result.Value as GenericWrapper<Guid>).Value);
        }

        [TestMethod]
        public void CreateResponseComplexType()
        {
            var passwordCriteria = new PasswordCriteria();
            var result = ControllerHelper.CreateResponse(passwordCriteria);

            Assert.IsInstanceOfType(result.Value, typeof(PasswordCriteria));
            Assert.AreSame(passwordCriteria, result.Value);
        }

        [TestMethod]
        public void CreateErrorResponse()
        {
            var status = HttpStatusCode.BadRequest;
            var code = "TestCode";

            var result = ControllerHelper.CreateErrorResponse(status, code);

            Assert.IsInstanceOfType(result.Value, typeof(HttpErrorMessage));
            Assert.AreEqual(code, (result.Value as HttpErrorMessage).Code);
            Assert.AreEqual((int)status, result.StatusCode);
        }

        [DataTestMethod]
        [DataRow(true, typeof(bool))]
        [DataRow(true, typeof(Enum))]
        [DataRow(true, typeof(string))]
        [DataRow(true, typeof(decimal))]
        [DataRow(true, typeof(DateTime))]
        [DataRow(true, typeof(DateTimeOffset))]
        [DataRow(true, typeof(TimeSpan))]
        [DataRow(true, typeof(Guid))]
        [DataRow(true, typeof(DateTimeOffset))]
        [DataRow(true, typeof(int))]
        [DataRow(false, typeof(DBNull))]
        [DataRow(false, typeof(TestGenericClass<bool>))]
        [DataRow(false, typeof(TestGenericClass<PasswordCriteria>))]
        [DataRow(true, typeof(Nullable<bool>))]
        public void IsSimpleType(bool expected, Type type)
        {
            var actual = ControllerHelper.IsSimpleType(type);
            Assert.AreEqual(expected, actual);
        }

        class TestGenericClass<T>
        {
        }
    }
}
