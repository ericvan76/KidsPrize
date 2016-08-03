using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables("ASPNETCORE_")
                .Build();
            var env = config.GetValue<string>("ASPNETCORE_ENVIRONMENT");

            var host = new WebHostBuilder()
                .UseKestrel(opts=>{
                    if (env == EnvironmentName.Production)
                    {
                        opts.UseHttps(new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "idsvr.crt")));
                    }
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
