using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Security.Authentication;
using System.Threading.Tasks;
using JHipsterNet.Web.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Hosting;
using Plainly.Infrastructure.Data;
using Serilog.Sinks.Syslog;
using ILogger = Serilog.ILogger;

namespace Plainly
{
    public class Program: IDesignTimeDbContextFactory<AppDbContext>
    {
        private const string SerilogSection = "Serilog";
        private const string SyslogPort = "SyslogPort";
        private const string SyslogUrl = "SyslogUrl";
        private const string SyslogAppName = "SyslogAppName";

        public static async Task<int> Main(string[] args)
        {
            // throwaway logger, pre-configuration load
            Log.Logger = new LoggerConfiguration()
                .CreateLogger();
            
            try
            {
                Log.Information("Starting web host");
                
                IHost host = Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((webHostBuilderContext, builder) =>
                    {
                        string envName = webHostBuilderContext.HostingEnvironment.EnvironmentName;
                        builder = ConfigureSettings(envName, builder);
                        Log.Logger = CreateLogger(builder.Build());
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    })
                    .Build();
                
                await host.RunAsync();

                return 0;
            }
            catch (Exception ex)
            {
                // Use ForContext to give a context to this static environment (for Serilog LoggerNameEnricher).
                Log.ForContext<Program>().Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        
        
        public static IConfigurationBuilder ConfigureSettings(string envName, IConfigurationBuilder builder)
        {
            return builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("appsettings.yaml", false)
                .AddYamlFile($"appsettings.{envName}.yaml", false)
                .AddEnvironmentVariables();
        }
        

        /// <summary>
        /// Create application logger from configuration.
        /// </summary>
        /// <returns></returns>
        private static ILogger CreateLogger(IConfiguration appConfiguration)
        {
            int port = 6514;

            // for logger configuration
            // https://github.com/serilog/serilog-settings-configuration
            if (appConfiguration.GetSection(SerilogSection)[SyslogPort] != null)
            {
                if (int.TryParse(appConfiguration.GetSection(SerilogSection)[SyslogPort], out int portFromConf))
                {
                    port = portFromConf;
                }
            }

            string url = appConfiguration.GetSection(SerilogSection)[SyslogUrl] != null
                ? appConfiguration.GetSection(SerilogSection)[SyslogUrl]
                : "localhost";
            
            string appName = appConfiguration.GetSection(SerilogSection)[SyslogAppName] != null
                ? appConfiguration.GetSection(SerilogSection)[SyslogAppName]
                : "JhipsterApp";
            
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.With<LoggerNameEnricher>()
                .WriteTo.TcpSyslog(url, port, appName, FramingType.OCTET_COUNTING, SyslogFormat.RFC5424, Facility.Local0, SslProtocols.None)
                .ReadFrom.Configuration(appConfiguration);

            return loggerConfiguration.CreateLogger();
        }

        public AppDbContext CreateDbContext(string[] args)
        {
            string envName = args[0];
            
            string connectionString = ConfigureSettings(envName, new ConfigurationBuilder())
                .Build()
                .GetConnectionString("AppDbContext");

            var opts = new DbContextOptionsBuilder<AppDbContext>();
            opts.UseNpgsql(connectionString,
                    npgsqlOpts =>
                    {
                        npgsqlOpts.MigrationsHistoryTable("__ef_migrations_history");
                    })
                .UseSnakeCaseNamingConvention(); 

            return new AppDbContext(opts.Options);
        }
    }
}
