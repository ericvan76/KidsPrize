using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Scrutor;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using KidsPrize.Http.Mvc;
using KidsPrize.Http.Jwt;
using Microsoft.AspNetCore.Rewrite;
using EasyVersioning.AspNetCore.Mvc;
using EasyVersioning.AspNetCore.Swagger;

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
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables("kidsprize:");

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
                    var policyBuilder = new AuthorizationPolicyBuilder();
                    policyBuilder.AddAuthenticationSchemes(new[] { "Bearer" });
                    policyBuilder.RequireAuthenticatedUser();
                    opts.Filters.Add(new AuthorizeFilter(policyBuilder.Build()));
                    opts.Filters.Add(new ModelStateValidActionFilter());
                    opts.Conventions.Insert(0, new VersionPrefixConvention());
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

            services.AddSingleton<IConfigurationRoot>(Configuration);

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Command))
                .AddClasses(cfg => cfg.Where(t => t.Name.EndsWith("Handler") || t.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
            );

            services.AddSwaggerGen(opts =>
            {
                opts.SetupVersionedDocs("KidsPrize API");
                opts.DescribeAllEnumsAsStrings();
                opts.MapType<Guid>(() => new Schema() { Type = "string", Format = "uuid" });
                opts.MapType<DateTime>(() => new Schema() { Type = "string", Format = "date" });
            });

            services.AddApiVersioning();

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
            var jwtOptions = new JwtBearerOptions()
            {
                Authority = authOptions.GetValue<string>("Authority"),
                Audience = authOptions.GetValue<string>("Audience"),
            };
            jwtOptions.SecurityTokenValidators.Clear();
            jwtOptions.SecurityTokenValidators.Add(new OverridedJwtSecurityTokenHandler());
            app.UseJwtBearerAuthentication(jwtOptions);

            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SetupEndpoints());

            // rewrite unversioned to v1
            app.UseRewriter(new RewriteOptions().AddRewrite(@"^(?!v\d+/)(.*)", "v1/$1", skipRemainingRules: true));
            app.UseMvc();

        }
    }
}