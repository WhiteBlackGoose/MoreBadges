using System.Text.Json;

public static class Serializer
{
    public static string Encode<T>(T o)
        => JsonSerializer.Serialize(o);

    public static T Decode<T>(string text)
        => JsonSerializer.Deserialize<T>(text) ?? throw new("Should not be null");
}

public sealed record CacheObject(string Value, DateTime Timestamp);

public sealed record SignedKeyObject(string Hostname, string Key);