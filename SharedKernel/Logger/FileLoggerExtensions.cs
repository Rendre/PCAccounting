using DB.Entities;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Logger;

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
    {
        var directory = new DirectoryInfo(Environment.CurrentDirectory).Parent?.Parent?.Parent?.Parent;
        var pathForSaveFile = directory + "\\log\\";
        builder.AddProvider(new FileLoggerProvider(pathForSaveFile));
        return builder;
    }

    //public static Computer KEK(this Computer computer)
    //{
    //    computer.ID = 0;
    //    return computer;
    //}
}