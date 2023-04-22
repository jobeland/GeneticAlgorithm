using Microsoft.Extensions.Logging;

namespace GeneticAlgorithm.Core;

public static class ConsoleLogger
{
    private static Lazy<ILoggerFactory> _lazyLoggerFactory = new(() => ConfigureLoggerFactory());

    private static ILoggerFactory _loggerFactory => _lazyLoggerFactory.Value;

    public static ILoggerFactory ConfigureLoggerFactory()
    {
        ILoggerFactory factory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        return factory;
    }

    public static ILogger GetLogger(string categoryName)
    {
        return _loggerFactory.CreateLogger(categoryName);
    }

    public static ILogger<T> GetLogger<T>()
    {
        return _loggerFactory.CreateLogger<T>();
    }
}