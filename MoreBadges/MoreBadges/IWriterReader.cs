using NuGet.Common;

public interface IWriterReader
{
    void Write(string host, string key, string value);

    string? Read(string host, string key);
}

public sealed record DefaultWriterReader(string Path) : IWriterReader
{
    public void Write(string host, string key, string value)
    {
        var path = System.IO.Path.Combine(Path, host, key);
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path) ?? throw new());
        File.WriteAllText(path, value);
    }

    public string? Read(string host, string key)
    {
        var path = System.IO.Path.Combine(Path, host, key);
        if (File.Exists(path))
            return File.ReadAllText(path);
        else
            return null;
    }
}

public sealed record DebugWriterReader(string Path, ILogger Logger) : IWriterReader
{
    private readonly DefaultWriterReader defWR = new(Path);

    public void Write(string host, string key, string value)
    {
        Logger.LogInformation($"Requested to write. Host: {host}. Key: {key}. Value: {value}");
        defWR.Write(host, key, value);
    }

    public string? Read(string host, string key)
    {
        var res = defWR.Read(host, key);
        if (res is not null)
            Logger.LogInformation($"Found cache on {host}:{key}");
        return res;
    }
}
