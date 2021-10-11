using BusinessLayer.Context;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BusinessLayer.Test
{
    [TestClass]
    public class ContextManagerTest
    {
        [TestMethod]
        public void BeginAndCommitTransactionCalled()
        {
            var contextMock = new Mock<QuizContext>(MockBehavior.Loose, new DbContextOptionsBuilder<QuizContext>().Options);
            var dbMock = new Mock<Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade>(MockBehavior.Loose, contextMock.Object);
            var beginTransactionCalled = false;
            var commitCalled = false;
            var rollbackCalled = false;
            var disposeTransactionCalled = false;
            var transactionMock = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();

            dbMock.Setup(c => c.BeginTransaction()).Returns(() =>
            {
                beginTransactionCalled = true;
                return transactionMock.Object;
            });

            transactionMock.Setup(c => c.Commit()).Callback(() =>
            {
                commitCalled = true;
            });

            transactionMock.Setup(c => c.Rollback()).Callback(() =>
            {
                rollbackCalled = true;
            });

            transactionMock.Setup(c => c.Dispose()).Callback(() =>
            {
                disposeTransactionCalled = true;
            });

            contextMock.SetupGet(c => c.Database).Returns(dbMock.Object);

            using (contextMock.Object)
            {
                var sut = new Implementations.ContextManager(contextMock.Object, Mock.Of<ILogManager>());

                sut.BeginTransaction();
                sut.Commit();
            }

            Assert.IsTrue(beginTransactionCalled);
            Assert.IsTrue(commitCalled);
            Assert.IsFalse(rollbackCalled);
            Assert.IsTrue(disposeTransactionCalled);
        }

        [TestMethod]
        public void BeginAndRollbackTransactionCalled()
        {
            var contextMock = new Mock<QuizContext>(MockBehavior.Loose, new DbContextOptionsBuilder<QuizContext>().Options);
            var dbMock = new Mock<Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade>(MockBehavior.Loose, contextMock.Object);
            var beginTransactionCalled = false;
            var commitCalled = false;
            var rollbackCalled = false;
            var disposeTransactionCalled = false;
            var transactionMock = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();

            dbMock.Setup(c => c.BeginTransaction()).Returns(() =>
            {
                beginTransactionCalled = true;
                return transactionMock.Object;
            });

            transactionMock.Setup(c => c.Commit()).Callback(() =>
            {
                commitCalled = true;
            });

            transactionMock.Setup(c => c.Rollback()).Callback(() =>
            {
                rollbackCalled = true;
            });

            transactionMock.Setup(c => c.Dispose()).Callback(() =>
            {
                disposeTransactionCalled = true;
            });

            contextMock.SetupGet(c => c.Database).Returns(dbMock.Object);

            using (contextMock.Object)
            {
                var sut = new Implementations.ContextManager(contextMock.Object, Mock.Of<ILogManager>());

                sut.BeginTransaction();
                sut.Rollback();
            }

            Assert.IsTrue(beginTransactionCalled);
            Assert.IsFalse(commitCalled);
            Assert.IsTrue(rollbackCalled);
            Assert.IsTrue(disposeTransactionCalled);
        }
    }
}
