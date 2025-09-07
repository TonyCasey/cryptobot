using System;
using System.Reflection;
using Api.CryptoBot.Data;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using CryptoBot.ExchangeEngine.API.Exchanges;
using CryptoBot.Model.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.CryptoBot
{
    public class Startup
    {
        private static ILogger _logger;

        public IConfigurationRoot Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddAutoMapper(typeof(Startup));

            string connectionString = Configuration.GetConnectionString("CryptoBotConnection");
            services.AddDbContext<CryptoBotApiDbContext>(options => options.UseSqlServer(connectionString, sqlServerOptionsAction:
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(1.2),
                        errorNumbersToAdd: null);
                }));

            BuildSwaggerService(services);

            AddVersioning(services);

            services.AddMvc(options =>
                options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.StringOutputFormatter>())
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
            

            ApplicationContainer = BuildAutofac(services);
            services.AddMvc(options => options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.StringOutputFormatter>());

            services.AddScoped<IExchangeApi, ExchangeAPI>();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(Startup));

#if DEBUG
            app.UseDeveloperExceptionPage();
#endif 
            app.UseMvcWithDefaultRoute();

            app.UseCors("CorsPolicy");

            BuildSwagger(app);

        }

        private void BuildSwagger(IApplicationBuilder app)
        {
            // https://github.com/domaindrivendev/Swashbuckle

            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
#if DEBUG
            app.UseSwaggerUI();
#endif 
        }

        private static void BuildSwaggerService(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "CryptoBot.Api", Version = "v1" });
                
                var xmlDocFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CryptoBot.Api.xml");
                if (System.IO.File.Exists(xmlDocFile))
                    c.IncludeXmlComments(xmlDocFile);
            });
        }

        private IContainer BuildAutofac(IServiceCollection services)
        {
            // http://docs.autofac.org/en/latest/integration/owin.html

            if (services == null) throw new ArgumentNullException(nameof(services));

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(GetType().GetTypeInfo().Assembly); // autoscan the current assembly, find all modules and load them
            builder.Populate(services);
            

            return builder.Build();
        }

        private static void AddVersioning(IServiceCollection services)
        {
            // https://github.com/Microsoft/aspnet-api-versioning/wiki/Versioning-via-the-URL-Path

            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddApiVersioning();
        }
    }
}
