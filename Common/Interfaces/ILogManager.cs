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
        /// <param name="exc"></param>
        /// <returns></returns>
        string GetExceptionDetails(Exception exc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="level"></param>
        void AddLog(string logMessage, LogEventLevel level = LogEventLevel.Information);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exc"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="level"></param>
        void AddLog(Exception exc, string messageTemplate = "", LogEventLevel level = LogEventLevel.Error);
    }
}
