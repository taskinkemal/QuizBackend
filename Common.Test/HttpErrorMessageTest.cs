using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Test
{
    [TestClass]
    public class HttpErrorMessageTest
    {
        [TestMethod]
        public void HttpErrorMessageSetsCorrectCode()
        {
            const string code = "MyErrorCode";

            var message = new HttpErrorMessage(code);

            Assert.AreEqual(code, message.Code);
        }
    }
}
