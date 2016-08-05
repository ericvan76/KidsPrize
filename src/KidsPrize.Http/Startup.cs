using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Bus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger.Model;
using KidsPrize.Http.Extensions;
using KidsPrize.Http.Bus;
using KidsPrize.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using NLog.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace KidsPrize.Http
{
    public class Startup
    {
        private readonly MapperConfiguration _mapperConfgiuration;
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            _mapperConfgiuration = new MapperConfiguration(cfg =>
                cfg.AddProfile(new Resources.MappingProfile()));

            _environment = env;
        }

        public IConfigurationRoot Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions()
                .Configure<DefaultTasks>(Configuration.GetSection("DefaultTasks"));

            services.AddMvc()
                .AddMvcOptions(opts =>
                {
                    opts.Filters.Add(new ModelStateValidActionFilter());
                    var policyBuilder = new AuthorizationPolicyBuilder();
                    policyBuilder.AddAuthenticationSchemes(new[] { "Bearer" });
                    policyBuilder.RequireAuthenticatedUser();
                    opts.Filters.Add(new AuthorizeFilter(policyBuilder.Build()));
                })
                .AddJsonOptions(opts =>
                {
                    opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddMemoryCache();

            // Add framework services.
            services.AddDbContext<KidsPrizeContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("KidsPrize.Http"));
            });

            // AutoMapper
            services.AddSingleton<IMapper>(s => _mapperConfgiuration.CreateMapper());

            // Add IBus
            services.AddSingleton<IBus, SimpleBus>();

            // Add Services & Handlers
            services.AutoRegistration();

            services.AddSwaggerGen(opts =>
            {
                opts.SingleApiVersion(new Info()
                {
                    Version = "v1",
                    Title = "KidsPrize API",
                    Description = "",
                    TermsOfService = ""
                });
                opts.DescribeAllEnumsAsStrings();
                opts.CustomSchemaIds(t => t.FullName);
                opts.MapType<Guid>(() => new Schema() { Type = "string", Format = "uuid" });
                opts.MapType<DateTime>(() => new Schema() { Type = "string", Format = "date" });
            });

            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // NLog
            loggerFactory.AddNLog();
			_environment.ConfigureNLog(System.IO.Path.Combine(_environment.ContentRootPath, "nlog.config"));

            // DbContext initialise
            var context = app.ApplicationServices.GetService<KidsPrizeContext>();
            context.Database.Migrate();

            // Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var idsvrOptions = Configuration.GetSection("IdentityServer");
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions()
            {
                Authority = idsvrOptions.GetValue<string>("Authority"),
                RequireHttpsMetadata = false; //!env.IsDevelopment(),
                ScopeName = "api1",
                AutomaticAuthenticate = true
            });

            app.UseMvc();

            if (_environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUi(swaggerUrl: $"/swagger/v1/swagger.json");
            }
        }
    }

    public class ModelStateValidActionFilter : IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
            {
                return next();
            }
            context.Result = new BadRequestObjectResult(context.ModelState);
            return Task.CompletedTask;
        }
    }

}