using System.Collections.Specialized;

public abstract class BaseBadgeInfoGetter
{
    public abstract Task<string?> RespondToRequest(NameValueCollection args);
}