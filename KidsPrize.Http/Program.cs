using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace KidsPrize.Http
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var productVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            Log.Logger = new LoggerConfiguration()
                // Suppress EF Core, which logs queries at info, to warning
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.WithProperty("ProductName", "KidsPrize API")
                .Enrich.WithProperty("ProductVersion", productVersion)
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.FromLogContext()
                .WriteTo.Async(a => a.Console(new RenderedCompactJsonFormatter()), blockWhenFull: true)
                .CreateLogger();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build())
            .ConfigureAppConfiguration(b => b.AddEnvironmentVariables("kidsprize:"))
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseStartup<Startup>()
            .UseSerilog()
            .Build();
    }

}