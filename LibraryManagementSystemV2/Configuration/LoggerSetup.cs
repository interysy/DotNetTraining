using Serilog;
using Microsoft.Extensions.Configuration;
using ILogger = Serilog.ILogger;

namespace LibraryManagementSystemV2.Configuration
{
    public static class LoggerSetup
    {
        const string LogFilePrefix = "logs/log_run_";
        const string LogFileSuffix = ".txt";
        const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} (App: {ApplicationName}, Env: {Environment}, Machine : {MachineName}, IP : {ClientIp}){NewLine}{Exception}";


        public static ILogger SetUpLogger(IConfiguration configuration)
        {
            System.IO.Directory.CreateDirectory("logs");

            string runName = LogFilePrefix + currentDateAsName() + LogFileSuffix;

            string? environment = configuration["SerilogEnrichers:Environment"];
            string? appName = configuration["SerilogEnrichers:ApplicationName"];

            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .Enrich.WithProperty("Environment", environment ?? "Unknown")
                        .Enrich.WithProperty("ApplicationName", appName ?? "Unknown")
                        .Enrich.WithMachineName()
                        .Enrich.WithClientIp()
                        .WriteTo.Console(outputTemplate: OutputTemplate)
                        .WriteTo.File(runName, outputTemplate: OutputTemplate)
                        .CreateLogger();

            return Log.Logger;
        }

        public static string currentDateAsName()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
        }
    }
}
