using Microsoft.Extensions.Logging;

namespace SharedKernel.Logger;

public static class FileLoggerExtensions
{
    public static void AddFile(this ILoggingBuilder builder)
    {
        //var directory = new DirectoryInfo(Environment.CurrentDirectory).Parent?.Parent?.Parent?.Parent;
        var directory = new DirectoryInfo(Environment.CurrentDirectory).Parent;
        var pathForSaveFile = directory + "\\log.txt";
        builder.AddProvider(new FileLoggerProvider(pathForSaveFile));
    }

    //public static Computer KEK(this Computer computer)
    //{
    //    computer.ID = 0;
    //    return computer;
    //}
}