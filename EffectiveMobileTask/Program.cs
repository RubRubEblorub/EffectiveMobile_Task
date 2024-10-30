using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace EffectiveMobileTask;

class Program
{
    static void Main(string[] args)
    {
        var logger = LogManager.GetCurrentClassLogger();
        
        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            
            var logConfig = new NLog.Config.LoggingConfiguration();
            var logFilePath = Path.Combine(Environment.CurrentDirectory, "logs.txt");

            var fileTarget = new NLog.Targets.FileTarget("logfile")
            {
                FileName = logFilePath,
                Layout = "${longdate} —— ${level} —— ${logger} —— ${message} ${all-event-properties} ${exception:format=tostring}"
            };

            logConfig.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, fileTarget);

            LogManager.Configuration = logConfig;
            
            var services = new ServiceCollection()
                .AddLogging(loggingBuiler =>
                {
                    loggingBuiler.ClearProviders();
                    loggingBuiler.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    loggingBuiler.AddNLog(config);
                })
                .AddTransient<DataBase>()
                .AddTransient<ConsoleInterface>();

            var serviceProvider = services.BuildServiceProvider();
            
            serviceProvider.GetService<DataBase>().CreateDataBaseJsonFile();
            serviceProvider.GetService<ConsoleInterface>().InitConsole();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Stopped program because of exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}