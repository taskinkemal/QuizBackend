using BusinessLayer.Context;
using Common.Interfaces;


namespace BusinessLayer.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ManagerBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected QuizContext Context { get; }

        /// <summary>
        /// 
        /// </summary>
        protected ILogManager LogManager { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logManager"></param>
        protected ManagerBase(QuizContext context, ILogManager logManager)
        {
            Context = context;
            LogManager = logManager;
        }
    }
}
