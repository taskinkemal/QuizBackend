using System;
using Common.Interfaces;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Common.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LogAdapter : ILogAdapter
    {
        private readonly ILogger logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public LogAdapter(IConfiguration configuration)
        {
            this.logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exc"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="logArguments"></param>
        public void Write(LogEventLevel level, Exception exc, string messageTemplate, params object[] logArguments)
        {
            logger.Write(level, exc, messageTemplate, logArguments);
        }
    }
}
