using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

public sealed record NeatNumberPair(string Normal, string Short)
{
    public static NeatNumberPair FromLong(long number)
    {
        var shortMessage = number switch
        {
            > 1_000_000_000 => $"{number / 100_000_000 / 10.0:F1}B",
            > 1_000_000     => $"{number / 100_000     / 10.0:F1}M",
            > 1_000         => $"{number / 100         / 10.0:F1}K",
            var other       => $"{number}"
        };
        return new(number.ToString(), shortMessage);
    }
}

public sealed class NugetGetDownloadsInfo : GetInfoAboutBadge<string, NeatNumberPair>
{
    private readonly ILogger logger;

    public NugetGetDownloadsInfo(IWriterReader writerReader, ILogger logger)
        : base("nugetDownloadInfo", writerReader, TimeSpan.FromHours(12))
    {
        this.logger = logger;
    }

    protected override NeatNumberPair GetInfoActive(string parameters)
    {
        CancellationToken cancellationToken = CancellationToken.None;

        SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        PackageSearchResource resource = repository.GetResource<PackageSearchResource>();
        SearchFilter searchFilter = new SearchFilter(includePrerelease: true);

        IEnumerable<IPackageSearchMetadata> results = resource.SearchAsync(
            $"owner:{parameters}",
            searchFilter,
            skip: 0,
            take: 20,
            logger,
            cancellationToken).Result;

        var res = results.Select(r => r.DownloadCount).Sum() ?? throw new("Can't be null");
        return NeatNumberPair.FromLong(res);
    }

    protected override string EncodeKey(string key)
        => key;
}