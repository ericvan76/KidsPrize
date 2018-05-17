using AutoMapper;
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
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Rewrite;
using EasyVersioning.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using KidsPrize.Services;

namespace KidsPrize.Http
{
    public class Startup
    {
        private readonly MapperConfiguration _mapperConfiguration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _mapperConfiguration = new MapperConfiguration(cfg =>
                cfg.AddProfile(new MappingProfile()));
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
                    policyBuilder.AddAuthenticationSchemes(new[] { "Bearer" });
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
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<KidsPrizeContext>(builder =>
            {
                builder.UseNpgsql(connectionString, options =>
                {
                    options.MigrationsAssembly(migrationsAssembly);
                    options.MigrationsHistoryTable("__MigrationHistory", "KidsPrize");
                });
            });

            services.AddSingleton<IConfiguration>(Configuration);

            // AutoMapper
            services.AddSingleton<IMapper>(s => _mapperConfiguration.CreateMapper());
            services.AddScoped<IChildService, ChildService>();
            services.AddScoped<IScoreService, ScoreService>();
            services.AddScoped<IRedeemService, RedeemService>();

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
            // DbContext initialise
            InitializeDatabase(app);

            // http://stackoverflow.com/questions/38153044/how-to-force-an-https-callback-using-microsoft-aspnetcore-authentication-google
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SetupEndpoints());

            // rewrite unversioned to v1
            app.UseRewriter(new RewriteOptions().AddRewrite(@"^(?!v\d+/)(.*)", "v1/$1", skipRemainingRules: true));
            app.UseMvc();

        }

        private static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<KidsPrizeContext>().Database.Migrate();
            }
        }
    }
}