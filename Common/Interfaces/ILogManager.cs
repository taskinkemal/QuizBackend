using Serilog.Events;
using System;

namespace Common.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILogManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="level"></param>
        /// <param name="logArguments"></param>
        void AddLog(LogCategory category, string messageTemplate, LogEventLevel level, params object[] logArguments);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="exc"></param>
        /// <param name="logArguments"></param>
        void AddLog(LogCategory category, Exception exc, params object[] logArguments);
    }
}
