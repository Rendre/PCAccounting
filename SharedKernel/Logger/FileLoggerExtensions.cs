using DB.Entities;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Logger;

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder,
        string filePath)
    {
        builder.AddProvider(new FileLoggerProvider(filePath));
        return builder;
    }

    //public static Computer KEK(this Computer computer)
    //{
    //    computer.ID = 0;
    //    return computer;
    //}
}