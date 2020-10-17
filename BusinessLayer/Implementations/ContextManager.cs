using BusinessLayer.Context;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace BusinessLayer.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class ContextManager : ManagerBase, IContextManager
    {

        /// <summary>
        /// 
        /// </summary>
        private IDbContextTransaction transaction;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logManager"></param>
        public ContextManager(QuizContext context, ILogManager logManager) : base(context, logManager)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void BeginTransaction()
        {
            transaction = Context.Database.BeginTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Rollback()
        {
            transaction.Rollback();
            transaction.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            transaction.Commit();
            transaction.Dispose();
        }
    }
}
