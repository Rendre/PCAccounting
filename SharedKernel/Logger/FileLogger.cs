using Microsoft.Extensions.Logging;

namespace SharedKernel.Logger;

public class FileLogger : ILogger, IDisposable
{
    private readonly string _filePath;
    private static readonly object Lock = new();
    public FileLogger(string path)
    {
        _filePath = path;
    }
    public IDisposable BeginScope<TState>(TState state)
    {
        return this;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        //return logLevel == LogLevel.Trace;
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter != null)
        {
            lock (Lock)
            {
                File.AppendAllText(_filePath, formatter(state, exception) + Environment.NewLine);
            }
        }
    }

    public void Dispose()
    {
    }
}