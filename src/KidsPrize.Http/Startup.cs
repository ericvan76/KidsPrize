using System;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using KidsPrize.Http.Services;
using KidsPrize.Repository.Npgsql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddMvcOptions(opts =>
                {
                    var policyBuilder = new AuthorizationPolicyBuilder();
                    policyBuilder.AddAuthenticationSchemes(new[] { "Bearer" });
                    policyBuilder.RequireAuthenticatedUser();
                    opts.Filters.Add(new AuthorizeFilter(policyBuilder.Build()));
                    opts.Filters.Add(new ModelStateValidActionFilter());
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
                opts.SwaggerDoc("v1", new Info
                {
                    Title = "KidsPrize API",
                    Version = "v1"
                });

                opts.DescribeAllEnumsAsStrings();
                opts.MapType<Guid>(() => new Schema() { Type = "string", Format = "uuid" });
                opts.MapType<DateTime>(() => new Schema() { Type = "string", Format = "date" });

                opts.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
            });

            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Disable startup migration to save cold start time.
            // app.UseNpgsqlDbContext();

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SwaggerEndpoint("v1/swagger.json", "KidsPrize API"));

            // app.UseHsts();
            // app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}