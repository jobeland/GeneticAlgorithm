using Microsoft.Extensions.Logging;

namespace GeneticAlgorithms.Core;

public static class ConsoleLogger
{
    private static readonly Lazy<ILoggerFactory> _lazyLoggerFactory = new(() => ConfigureLoggerFactory());

    public static ILoggerFactory LoggerFactory => _lazyLoggerFactory.Value;

    public static ILoggerFactory ConfigureLoggerFactory()
    {
        ILoggerFactory factory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        return factory;
    }

    public static ILogger GetLogger(string categoryName)
    {
        return LoggerFactory.CreateLogger(categoryName);
    }

    public static ILogger<T> GetLogger<T>()
    {
        return LoggerFactory.CreateLogger<T>();
    }
}