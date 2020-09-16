using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Text;
using Serilog.Core;
using Common.Interfaces;

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
        /// <param name="settings"></param>
        public LogManager(IOptions<Common.AppSettings> settings)
        {
            //var config = new LoggerConfiguration()
            //    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(settings.Value.SerilogUri))
            //    {
            //        AutoRegisterTemplate = true
            //    });

            //Log.Logger = config.CreateLogger();

            Log.Logger = new LoggerConfiguration().CreateLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exc"></param>
        /// <returns></returns>
        public string GetExceptionDetails(Exception exc)
        {
            var sb = new StringBuilder();
            sb.Append(exc.GetType());

            sb.Append(" Message: ");
            sb.Append(exc.Message);
            sb.Append(" StackTrace: ");
            sb.Append(exc.StackTrace);
            sb.Append(" Source: ");
            sb.Append(exc.Source);
            sb.Append(" InnerException: ");
            sb.Append(exc.InnerException);

            string[] keys;
            if (exc.Data.Keys.Count > 0)
            {
                keys = new string[exc.Data.Keys.Count];
                exc.Data.Keys.CopyTo(keys, 0);

                foreach (var key in keys)
                {
                    sb.Append(key);
                    sb.Append(" ");
                    sb.Append(exc.Data[key]);
                }
            }

            var innerExc = exc.InnerException;
            while (innerExc != null)
            {
                sb.Append(innerExc.Message + " ");
                sb.Append(innerExc.StackTrace + " ");
                sb.Append(innerExc.Source + " ");

                if (innerExc.Data.Keys.Count > 0)
                {
                    keys = new string[innerExc.Data.Keys.Count];
                    innerExc.Data.Keys.CopyTo(keys, 0);

                    foreach (var key in keys)
                    {
                        sb.Append(" ");
                        sb.Append(key);
                        sb.Append(" ");
                        sb.Append(innerExc.Data[key]);
                    }
                }

                innerExc = innerExc.InnerException;
            }

            return sb.ToString();
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
