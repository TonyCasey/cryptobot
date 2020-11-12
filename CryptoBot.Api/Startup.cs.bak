using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApi.template
{
    public class Startup
    {
        private static ILogger _logger;

        public IConfigurationRoot Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        public Startup(IHostingEnvironment env)
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
            services.AddMvc();
            BuildSwaggerService(services);

            ApplicationContainer = BuildAutofac(services);
            services.AddMvc(options => options.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.StringOutputFormatter>());

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            _logger = loggerFactory.CreateLogger(typeof(Startup));

#if DEBUG
            app.UseDeveloperExceptionPage();
#endif 
            app.UseMvcWithDefaultRoute();
            BuildSwagger(app);
        }

        private void BuildSwagger(IApplicationBuilder app)
        {
            // https://github.com/domaindrivendev/Swashbuckle

            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
#if DEBUG
            app.UseSwaggerUi();
#endif 
        }

        private static void BuildSwaggerService(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(c =>
            {
                var xmlDocFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "webApi.template.xml");
                if (System.IO.File.Exists(xmlDocFile))
                    c.IncludeXmlComments(xmlDocFile);
                else
                    // logger.LogDebug($"Swagger expected a xml doc file  at '{xmlDocFile}' which was not found.");

                c.DescribeAllEnumsAsStrings(); // if this is not enabled, enum values are treated as int's. probably not what you want
                c.SingleApiVersion(new Swashbuckle.Swagger.Model.Info() {Title = "webApi.template", Version = "v1"});
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
    }
}
