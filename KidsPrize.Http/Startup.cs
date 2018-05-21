using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using AutoMapper;
using EasyVersioning.AspNetCore;
using KidsPrize.Abstractions;
using KidsPrize.Http.Services;
using KidsPrize.Repository.Npgsql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace KidsPrize.Http
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            };

            services.AddMvc()
                .AddMvcOptions(opts =>
                {
                    var policyBuilder = new AuthorizationPolicyBuilder();
                    policyBuilder.AddAuthenticationSchemes(new [] { "Bearer" });
                    policyBuilder.RequireAuthenticatedUser();
                    opts.Filters.Add(new AuthorizeFilter(policyBuilder.Build()));
                    opts.Filters.Add(new ModelStateValidActionFilter());
                    opts.Conventions.Insert(0, new VersionPrefixConvention());
                });

            // Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication().AddJwtBearer(opt =>
            {
                var authOptions = Configuration.GetSection("JwtBearerOptions");
                opt.Authority = authOptions.GetValue<string>("Authority");
                opt.Audience = authOptions.GetValue<string>("Audience");
                opt.SecurityTokenValidators.Clear();
                opt.SecurityTokenValidators.Add(new OverridedJwtSecurityTokenHandler());
            });

            // Add DBContext
            services.AddNpgsqlDbContext(Configuration.GetConnectionString("DefaultConnection"));

            services.AddAutoMapper();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddScoped<IChildService, ChildService>();
            services.AddScoped<IScoreService, ScoreService>();
            services.AddScoped<IRedeemService, RedeemService>();

            // Swagger
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // http://stackoverflow.com/questions/38153044/how-to-force-an-https-callback-using-microsoft-aspnetcore-authentication-google
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            // DbContext initialise
            app.UseNpgsqlDbContext();

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SetupEndpoints());

            app.UseMvc();
        }
    }
}