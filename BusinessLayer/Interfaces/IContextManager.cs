using System.Threading.Tasks;
using Common.Interfaces;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContextManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void BeginTransaction();

        /// <summary>
        /// 
        /// </summary>
        void Rollback();

        /// <summary>
        /// 
        /// </summary>
        void Commit();
    }
}
