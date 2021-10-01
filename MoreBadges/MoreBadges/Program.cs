using NuGet.Common;
using System.Collections.Specialized;
using System.Net;

using var server = new HttpListener();

server.Prefixes.Add("http://+:11456/");

var cachePath = args.Length > 0 ? args[0] : "./caches";

var logger = new DebugLogger();
logger.LogInformation($"Using path {cachePath} for caching");
logger.LogInformation($"Absolute path for caching: {Path.GetFullPath(cachePath)}");


IWriterReader writerReader = new DebugWriterReader(cachePath, logger);

var runningTasks = new RunningTasksCollection();
var nugetBadgeInfo = new NugetGetDownloadsInfo(writerReader, logger);

server.Start();

while (true)
{
    var context = server.GetContext();
    var request = context.Request;
    var response = context.Response;
    var query = request.QueryString;
    if (request.Url?.AbsoluteUri.Contains("favicon.ico") ?? false)
        continue;

    BaseBadgeInfoGetter? runner = query["badge"] switch
    {
        "nugetdownloads" => nugetBadgeInfo,
        _ => null
    };

    if (runner is not null)
    {
        runningTasks.Add(ProcessRequest(response, query, runner));
        logger.LogInformation($"Success {request.Url}");
    }
    else
    {
        logger.LogInformation($"Error {request.Url}");
        if (query["badge"] is null)
            Server.Respond(Serializer.Encode(new { Error = "You should pass 'badge' parameter" }), response);
        else
            Server.Respond(Serializer.Encode(new { Error = $"Unrecognized badge {query["badge"]}" }), response);
    }

    logger.LogInformation($"Currently running tasks: {runningTasks.CountRunning()}");
}

#pragma warning disable CS0162 // Unreachable code detected
server.Stop();
#pragma warning restore CS0162 // Unreachable code detected

static async Task ProcessRequest(HttpListenerResponse response, NameValueCollection args, BaseBadgeInfoGetter badgeInfoGetter)
{
    var responseString = await badgeInfoGetter.RespondToRequest(args);
    if (responseString is null)
        return;
    Server.Respond(responseString, response);
}