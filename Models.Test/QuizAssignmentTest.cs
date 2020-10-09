using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;

namespace Models.Test
{
    [TestClass]
    public class QuizAssignmentTest
    {
        [TestMethod]
        public void QuizAssignmentConstructor()
        {
            var assignment = new QuizAssignment
            {
                Email = "myemail@email.com",
                QuizIdentityId = 55
            };

            Assert.AreEqual("myemail@email.com", assignment.Email);
            Assert.AreEqual(55, assignment.QuizIdentityId);
        }
    }
}
