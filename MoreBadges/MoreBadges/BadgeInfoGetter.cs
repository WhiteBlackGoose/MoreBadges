using NuGet.Common;

public abstract class BadgeInfoGetter<TKey, TValue> : BaseBadgeInfoGetter
{
    protected abstract Task<TValue> GetInfoActive(TKey parameters);
    protected abstract string EncodeKey(TKey key);

    private readonly string name;
    private readonly IWriterReader writerReader;
    private readonly TimeSpan cacheLifetime;
    protected readonly IExtendedLogger logger;

    public BadgeInfoGetter(string name, IWriterReader writerReader, TimeSpan cacheLifetime, IExtendedLogger logger)
    {
        this.name = name;
        this.writerReader = writerReader;
        this.cacheLifetime = cacheLifetime;
        this.logger = logger;
    }

    private string? GetValidatedValue(string key)
    {
        if (writerReader.Read(name, key) is not { } res)
        {
            logger.LogNegative($"Cache miss. No cache found for {key}");
            return null;
        }
        var obj = Serializer.Decode<CacheObject>(res);
        var cacheAge = DateTime.Now - obj.Timestamp;
        logger.LogInformation($"Cache as old as {cacheAge} was found");
        if (cacheAge < cacheLifetime)
        {
            logger.LogPositive($"Cache hit. It's younger than {cacheLifetime}, so using it");
            return obj.Value;
        }
        logger.LogNegative($"Cache miss");
        logger.LogInformation($"Too old for max cache life time of {cacheLifetime}");
        return null;
    }

    private string CacheAndReturnEncodedValue(string key, TValue value)
    {
        var encoded = Serializer.Encode(value);
        var cacheObj = new CacheObject(encoded, DateTime.Now);
        var encodedRes = Serializer.Encode(cacheObj);
        writerReader.Write(name, key, encodedRes);
        return encoded;
    }

    protected async Task<string> GetInfo(TKey parameters)
    {
        var key = EncodeKey(parameters);
        if (GetValidatedValue(key) is { } res)
            return res;
        var value = await GetInfoActive(parameters);
        return CacheAndReturnEncodedValue(key, value);
    }
}