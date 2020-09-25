using Serilog.Events;
using System;
using Common.Interfaces;
using System.Linq;

namespace Common.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class LogManager : ILogManager
    {
        private readonly ILogAdapter logAdapter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logAdapter"></param>
        public LogManager(ILogAdapter logAdapter)
        {
            this.logAdapter = logAdapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="level"></param>
        /// <param name="logArguments"></param>
        public void AddLog(LogCategory category, string messageTemplate, LogEventLevel level, params object[] logArguments)
        {
            AddLog(category, null, messageTemplate, level, logArguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="exc"></param>
        /// <param name="logArguments"></param>
        public void AddLog(LogCategory category, Exception exc, params object[] logArguments)
        {
            AddLog(category, exc, "Quiz exception", LogEventLevel.Error, logArguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="exc"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="level"></param>
        /// <param name="logArguments"></param>
        private void AddLog(LogCategory category, Exception exc, string messageTemplate, LogEventLevel level, params object[] logArguments)
        {
            var logParameters = CreateLogParameters(category);

            var pTemplate = messageTemplate ?? "";
            pTemplate += " {@LogParameters}";

            var list = logArguments;
            list = list.Concat(new[] { logParameters }).ToArray();

            logAdapter.Write(level, exc, pTemplate, list);
        }

        private static LogParameters CreateLogParameters(LogCategory category)
        {
            return new LogParameters
            {
                CategoryId = (int)category
            };
        }
    }
}
