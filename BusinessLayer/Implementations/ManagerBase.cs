using BusinessLayer.Context;


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
        /// <param name="context"></param>
        protected ManagerBase(QuizContext context)
        {
            Context = context;
        }
    }
}
