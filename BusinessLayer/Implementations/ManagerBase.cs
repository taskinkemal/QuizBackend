using BusinessLayer.Context;


namespace BusinessLayer.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ManagerBase
    {
        protected QuizContext Context;

        protected ManagerBase(QuizContext context)
        {
            Context = context;
        }
    }
}
