public abstract class GetInfoAboutBadge<TKey, TValue>
{
    protected abstract Task<TValue> GetInfoActive(TKey parameters);
    protected abstract string EncodeKey(TKey key);

    private readonly string name;
    private readonly IWriterReader writerReader;
    private readonly TimeSpan cacheLifetime;

    public GetInfoAboutBadge(string name, IWriterReader writerReader, TimeSpan cacheLifetime)
    {
        this.name = name;
        this.writerReader = writerReader;
        this.cacheLifetime = cacheLifetime;
    }

    private string? GetValidatedValue(string key)
    {
        if (writerReader.Read(name, key) is not { } res)
            return null;
        var obj = Serializer.Decode<CacheObject>(res);
        if (obj.Timestamp - DateTime.Now < cacheLifetime)
            return obj.Value;
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

    public async Task<string> GetInfo(TKey parameters)
    {
        var key = EncodeKey(parameters);
        if (GetValidatedValue(key) is { } res)
            return res;
        var value = await GetInfoActive(parameters);
        return CacheAndReturnEncodedValue(key, value);
    }
}