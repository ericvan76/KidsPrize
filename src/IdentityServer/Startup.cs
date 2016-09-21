using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Services;
using System.Collections.Generic;
using NLog.Extensions.Logging;
using IdentityServer4.Stores;
using IdentityServer4;
using IdentityServer4.Services;

namespace IdentityServer
{
    public class Startup
    {
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

            _environment = env;
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var certOptions = Configuration.GetSection("SigningCredential");
            var cert = new X509Certificate2(
                Path.Combine(_environment.ContentRootPath, certOptions.GetValue<string>("FileName")),
                certOptions.GetValue<string>("Password"));

            var builder = services.AddIdentityServer()
            .AddInMemoryStores()
            .SetSigningCredential(cert);

            // Options
            services.AddOptions()
                .Configure<List<ScopeOption>>(Configuration.GetSection("Scopes"))
                .Configure<List<ClientOption>>(Configuration.GetSection("Clients"));

            // Add framework services.
            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("IdentityServer"));
            });

            // for the UI
            services.AddMvc();
            services.AddTransient<IUserLoginService, UserLoginService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<IScopeStore, ScopeStore>();
            services.AddTransient<IClientStore, ClientStore>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // NLog
            loggerFactory.AddNLog();
			_environment.ConfigureNLog(System.IO.Path.Combine(_environment.ContentRootPath, "nlog.config"));

            app.UseDeveloperExceptionPage();

            // DbContext initialise
            var context = app.ApplicationServices.GetService<IdentityContext>();
            context.Database.Migrate();

            // Setup authentications
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });

            var googleOpts = Configuration.GetSection("GoogleOptions");
            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                DisplayName = "Google",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = googleOpts.GetValue<string>("ClientId"),
                ClientSecret = googleOpts.GetValue<string>("ClientSecret"),
                CallbackPath = new PathString(googleOpts.GetValue<string>("CallbackPath"))
            });

            var facebookOpts = Configuration.GetSection("FacebookOptions");
            app.UseFacebookAuthentication(new FacebookOptions
            {
                AuthenticationScheme = "Facebook",
                DisplayName = "Facebook",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = facebookOpts.GetValue<string>("ClientId"),
                ClientSecret = facebookOpts.GetValue<string>("ClientSecret"),
                CallbackPath = new PathString(facebookOpts.GetValue<string>("CallbackPath"))
            });

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}