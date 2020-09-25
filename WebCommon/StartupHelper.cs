using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Common;
using Common.Implementations;
using Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebCommon
{
    /// <summary>
    /// 
    /// </summary>
    public static class StartupHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void InjectDependencies(IServiceCollection services)
        {
            services.AddSingleton<ILogManager, LogManager>();
            services.AddSingleton<ILogAdapter, LogAdapter>();
            services.AddSingleton<IEmailManager, EmailManager>();

            services.AddScoped<IContextManager, ContextManager>();
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IQuizManager, QuizManager>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureAppSettings(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializerSettings"></param>
        public static void SetNewtonsoftSerializerSettings(JsonSerializerSettings serializerSettings)
        {
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            serializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
            serializerSettings.ContractResolver = new DefaultContractResolver();
            serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}
