using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Common;
using Common.Interfaces;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using BusinessLayer.Interfaces;
using BusinessLayer.Implementations;
using BusinessLayer.Context;
using Common.Implementations;

namespace WebApplication
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddCors(o => o.AddPolicy("DefaultPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            }));

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                }).AddMvcOptions(options => {
                    options.EnableEndpointRouting = false;
                });

            services.AddDbContext<QuizContext>(
                options => options.UseSqlServer(Configuration.GetSection("AppSettings").GetValue<string>("ConnectionString")), ServiceLifetime.Scoped);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.Configure<IISServerOptions>(options =>
            {
                // The Newtonsoft.json is not working without allowing synchronous IO.
                // And at the moment we cannot switch to System.Text.Json because the GraphQl libraries are still using Newtonsoft.
                // https://github.com/dotnet/aspnetcore/issues/8302
                options.AllowSynchronousIO = true;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                // The Newtonsoft.json is not working without allowing synchronous IO.
                // And at the moment we cannot switch to System.Text.Json because the GraphQl libraries are still using Newtonsoft.
                // https://github.com/dotnet/aspnetcore/issues/8302
                options.AllowSynchronousIO = true;
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                SetNewtonsoftSerializerSettings(options.SerializerSettings);
            });

            InjectDependencies(services);
        }

        private void InjectDependencies(IServiceCollection services)
        {
            services.AddSingleton<ILogManager, LogManager>();
            services.AddSingleton<IEmailManager, EmailManager>();

            services.AddScoped<IAuthManager, AuthManager>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("DefaultPolicy");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "api/{controller}/{id?}");
            });
        }

        private void SetNewtonsoftSerializerSettings(JsonSerializerSettings serializerSettings)
        {
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            serializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
            serializerSettings.ContractResolver = new DefaultContractResolver();
            serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}
