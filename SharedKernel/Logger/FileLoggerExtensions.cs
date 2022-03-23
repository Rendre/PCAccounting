using Microsoft.Extensions.Logging;

namespace SharedKernel.Logger;

internal static class FileLoggerExtensions
{
    public static ILoggerFactory AddFile(this ILoggerFactory factory,
        string filePath)
    {
        factory.AddProvider(new FileLoggerProvider(filePath));
        return factory;
    }

}