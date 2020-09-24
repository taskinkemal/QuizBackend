using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using BusinessLayer.Context;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using WebCommon;

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

            StartupHelper.ConfigureAppSettings(services, Configuration);

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
                options => options.UseSqlServer(Configuration.GetSection("AppSettings").GetValue<string>("ConnectionString")));

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ConfigureIIS(services);

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                StartupHelper.SetNewtonsoftSerializerSettings(options.SerializerSettings);
            });

            StartupHelper.InjectDependencies(services);

            ConfigureServiceSwagger(services);

            Environment.SetEnvironmentVariable("BASEDIR", PlatformServices.Default.Application.ApplicationBasePath);
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

            app.UseStaticFiles();

            app.UseCors("DefaultPolicy");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "api/{controller}/{id?}");
            });

            ConfigureAppSwagger(app);
        }

        private static void ConfigureIIS(IServiceCollection services)
        {
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
        }

        private static void ConfigureAppSwagger(IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => {
                    swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "V1 Docs");
                c.DocumentTitle = "Quiz API";
            });
        }

        private static void ConfigureServiceSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please insert the access token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });
            });

            services.ConfigureSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Quiz API",
                        Version = "v1",
                        Description = "Quiz Web API Documentation"
                    });

                AddXmlComments(options, "WebApplication", "BusinessLayer", "Common", "Models", "WebCommon");
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        private static void AddXmlComments(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options, params string[] projectNames)
        {
            foreach (var projectName in projectNames)
            {
                options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, projectName + ".xml"));
            }
        }
    }
}
