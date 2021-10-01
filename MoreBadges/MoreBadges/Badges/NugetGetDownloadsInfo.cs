using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.Collections.Specialized;
using System.Globalization;

public sealed record NugetInfo(string Normal, string Short, string NormalSplit, string Packages)
{
    public static NugetInfo FromLong(long number, int packageCount)
    {
        var shortMessage = number switch
        {
            > 1_000_000_000 => $"{number / 100_000_000 / 10.0:F1} B",
            > 1_000_000     => $"{number / 100_000     / 10.0:F1} M",
            > 1_000         => $"{number / 100         / 10.0:F1} K",
            _               => $"{number}"
        };
        // from https://stackoverflow.com/questions/17527847/how-would-i-separate-thousands-with-space-in-c-sharp
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        var normalSplit = number.ToString("#,0", nfi);
        return new(number.ToString(), shortMessage, normalSplit, packageCount.ToString());
    }
}

public sealed class NugetGetDownloadsInfo : BadgeInfoGetter<string, NugetInfo>
{
    public NugetGetDownloadsInfo(IWriterReader writerReader, IExtendedLogger logger)
        : base(
            "nugetDownloadInfo", 
            writerReader, 
            TimeSpan.FromHours(3),
            logger) { }

    protected override async Task<NugetInfo> GetInfoActive(string parameters)
    {
        var cancellationToken = CancellationToken.None;

        var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        var resource = await repository.GetResourceAsync<PackageSearchResource>();
        var searchFilter = new SearchFilter(includePrerelease: true);

        var results = await resource.SearchAsync(
            $"owner:{parameters}",
            searchFilter,
            skip: 0,
            take: 1000,
            logger,
            cancellationToken);

        var res = 0L;
        var count = 0;
        foreach (var r in results)
        {
            res += r.DownloadCount ?? throw new("Cannot be null");
            count++;
        }
        return NugetInfo.FromLong(res, count);
    }

    protected override string EncodeKey(string key)
        => key;

    public override async Task<string?> RespondToRequest(NameValueCollection args)
    {
        var user = args["user"];

        if (user is null)
        {
            logger.LogWarning("User is missing, skipping");
            return null;
        }

        var responseString = await GetInfo(user);
        return responseString;
    }
}