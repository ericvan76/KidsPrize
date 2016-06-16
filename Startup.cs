using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Models;
using KidsPrize.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.SwaggerGen.Generator;

namespace KidsPrize.Http
{
    public class Startup
    {
        private readonly MapperConfiguration mapperConfgiuration;
        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            mapperConfgiuration = new MapperConfiguration(cfg =>
                cfg.AddProfile(new Resources.MappingProfile()));
        }

        public IConfigurationRoot Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddIdentityServer();

            services.AddMvc()
                .AddMvcOptions(opts =>
                {
                    opts.Filters.Add(new ModelStateValidActionFilter());
                })
                .AddJsonOptions(opts =>
                {
                    opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddMemoryCache();

            // Add Entity Framework services to the services container.
            services.AddDbContext<KidsPrizeDbContext>(opts =>
               {
                   opts.UseSqlite("Filename=./kids-prize.db");
               }
            );

            // AutoMapper
            services.AddSingleton<IMapper>(s => mapperConfgiuration.CreateMapper());

            // Add services
            services.AddScoped<IChildService, ChildService>();
            services.AddScoped<IUserService, UserService>();

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
            });
            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();

            // Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = "/",
                RequireHttpsMetadata = false,
                ScopeName = "api1",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });


            // Identity Server
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "External",
                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });

            var googleOpts = Configuration.GetSection("IdentityServer:GoogleOptions");
            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                SignInScheme = "External",
                ClientId = googleOpts.GetValue<string>("ClientId"),
                ClientSecret = googleOpts.GetValue<string>("ClientSecret"),
                CallbackPath = new PathString("/connect/signin-google")
            });

            var facebookOpts = Configuration.GetSection("IdentityServer:FacebookOptions");
            app.UseFacebookAuthentication(new FacebookOptions
            {
                AuthenticationScheme = "Facebook",
                SignInScheme = "External",
                ClientId = facebookOpts.GetValue<string>("ClientId"),
                ClientSecret = facebookOpts.GetValue<string>("ClientSecret"),
                CallbackPath = new PathString("/connect/signin-facebook")
            });
            app.UseIdentityServer();

            app.UseMvc();

            app.UseSwaggerGen();
            app.UseSwaggerUi(swaggerUrl: $"/swagger/v1/swagger.json");
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