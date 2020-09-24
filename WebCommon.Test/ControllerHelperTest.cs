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
        public void CreateResponseSimpleType()
        {
            var result = ControllerHelper.CreateResponse(true);

            Assert.IsInstanceOfType(result.Value, typeof(GenericWrapper<bool>));
            Assert.IsTrue((result.Value as GenericWrapper<bool>).Value);
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
    }
}
