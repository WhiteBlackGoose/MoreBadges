using NuGet.Common;

public interface IExtendedLogger : ILogger
{
    void LogPositive(string data);

    void LogNegative(string data);
}

public sealed class DebugLogger : IExtendedLogger
{
    internal struct ChangeColor : IDisposable
    {
        private readonly ConsoleColor previous;
        public ChangeColor(ConsoleColor color)
        {
            previous = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public void Dispose()
        {
            Console.ForegroundColor = previous;
        }
    }

    public void LogPositive(string data)
    {
        using var _ = new ChangeColor(ConsoleColor.Green);
        LogDate();
        Console.WriteLine($"[good] {data}");
    }

    public void LogNegative(string data)
    {
        using var _ = new ChangeColor(ConsoleColor.Yellow);
        LogDate();
        Console.WriteLine($"[bad] {data}");
    }

    private void LogDate()
    {
        using var _ = new ChangeColor(ConsoleColor.DarkGray);
        Console.Write($"[{DateTime.Now:yyyy.MM.dd HH:mm:ss}]: ");
    }

    public void LogDebug(string data)
    {
        LogDate();
        Console.WriteLine($"[debug] {data}");
    }

    public void LogVerbose(string data)
    {
        LogDate();
        Console.WriteLine($"[verbose] {data}");
    }

    public void LogInformation(string data)
    {
        LogDate();
        Console.WriteLine($"[i] {data}");
    }

    public void LogMinimal(string data)
    {
        LogDate();
        Console.WriteLine($"[min] {data}");
    }

    public void LogWarning(string data)
    {
        using var _ = new ChangeColor(ConsoleColor.Yellow);
        LogDate();
        Console.WriteLine($"[warning] {data}");
    }

    public void LogError(string data)
    {
        LogDate();
        Console.WriteLine($"Logging error: {data}");
    }

    public void LogInformationSummary(string data)
    {
        LogDate();
        Console.WriteLine($"[summary] {data}");
    }

    public void Log(LogLevel level, string data)
    {
        LogDate();
        Console.WriteLine($"Logging {level}: {data}");
    }

    public Task LogAsync(LogLevel level, string data)
    {
        LogDate();
        Console.WriteLine($"Logging {level}: {data}");
        return Task.CompletedTask;
    }

    public void Log(ILogMessage message)
    {
        LogDate();
        Console.WriteLine($"Message: {message.Message}");
    }

    public Task LogAsync(ILogMessage message)
    {
        LogDate();
        Console.WriteLine($"Message: {message.Message}");
        return Task.CompletedTask;
    }
}
