using AutoMapper;
using KidsPrize.Http.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Scrutor;
using Swashbuckle.Swagger.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Threading.Tasks;
using System;

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
                cfg.AddProfile(new MappingProfile()));

            _environment = env;
        }

        public IConfigurationRoot Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {

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

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Add framework services.
            services.AddDbContext<KidsPrizeContext>(builder =>
            {
                builder.UseNpgsql(connectionString, options =>
                {
                    options.MigrationsAssembly(migrationsAssembly);
                    options.MigrationsHistoryTable("__MigrationHistory", "KidsPrize");
                });
            });

            // AutoMapper
            services.AddSingleton<IMapper>(s => _mapperConfgiuration.CreateMapper());

            // MediatR
            services.AddScoped<SingleInstanceFactory>(p => t => p.GetRequiredService(t));
            services.AddScoped<MultiInstanceFactory>(p => t => p.GetRequiredServices(t));
            services.AddScoped<IMediator, Mediator>();

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Command))
                .AddClasses(cfg => cfg.Where(t => t.Name.EndsWith("Handler") || t.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
            );

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

            if (_environment.IsProduction())
            {
                // http://stackoverflow.com/questions/38153044/how-to-force-an-https-callback-using-microsoft-aspnetcore-authentication-google
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedProto
                });
            }

            // Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Jwt Bearer
            var authOptions = Configuration.GetSection("JwtBearerOptions");
            app.UseJwtBearerAuthentication(new JwtBearerOptions()
            {
                Authority = authOptions.GetValue<string>("Authority"),
                Audience = authOptions.GetValue<string>("Audience")
            });

            app.UseMvc();

            if (Configuration.GetValue<bool>("EnableSwagger") == true)
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