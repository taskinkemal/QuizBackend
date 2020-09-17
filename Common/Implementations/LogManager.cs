using Serilog;
using Serilog.Events;
using System;
using Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Common.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class LogManager : ILogManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public LogManager(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="level"></param>
        public void AddLog(string logMessage, LogEventLevel level = LogEventLevel.Information)
        {
            Log.Write(level, logMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exc"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="level"></param>
        public void AddLog(Exception exc, string messageTemplate = "", LogEventLevel level = LogEventLevel.Error)
        {
            Log.Write(level, exc, messageTemplate ?? "Quiz exception");
        }
    }
}
