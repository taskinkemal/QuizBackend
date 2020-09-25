using System;
using Serilog.Events;

namespace Common.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILogAdapter : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exc"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="logArguments"></param>
        void Write(LogEventLevel level, Exception exc, string messageTemplate, params object[] logArguments);
    }
}
