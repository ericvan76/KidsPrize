using System.IdentityModel.Tokens.Jwt;
using KidsPrize.Data;
using KidsPrize.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KidsPrize
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
            services.AddControllers();

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

            // Add Authorization
            services.AddAuthorization(opts =>
            {
                opts.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(new[] { "Bearer" })
                    .RequireAuthenticatedUser()
                    .Build();
            });

            // Add DBContext
            services.AddNpgsqlDbContext(Configuration.GetConnectionString("DefaultConnection"));

            services.AddScoped<IChildService, ChildService>();
            services.AddScoped<IScoreService, ScoreService>();
            services.AddScoped<IRedeemService, RedeemService>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "KidsPrize API", Version = "v1" });
            // });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }

            // app.UseSwagger();
            // app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "KidsPrize API V1"));

            //app.UseHttpsRedirection();
            app.UseNpgsqlDbContext();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}