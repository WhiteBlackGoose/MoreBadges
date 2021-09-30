using NuGet.Common;
using System.Collections.Specialized;
using System.Net;

using var server = new HttpListener();

server.Prefixes.Add("http://+:11456/");

var cachePath = args.Length > 0 ? args[0] : "./caches";

ILogger logger = new DebugLogger();
logger.LogInformation($"Using path {cachePath} for caching");

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

    var task = query["badge"] switch
    {
        "nugetdownloads" => NugetRespondToRequest(response, query, logger, nugetBadgeInfo),
        _ => null
    };

    if (task is not null)
    {
        runningTasks.Add(task);
        logger.LogInformation($"Success {request.Url}");
    }
    else
    {
        logger.LogInformation($"Error {request.Url}");
        if (query["badge"] is null)
            ProcessResponse(Serializer.Encode(new { Error = "You should pass 'badge' parameter" }), response);
        else
            ProcessResponse(Serializer.Encode(new { Error = $"Unrecognized badge {query["badge"]}" }), response);
    }

    logger.LogInformation($"Currently running tasks: {runningTasks.CountRunning()}");
}

#pragma warning disable CS0162 // Unreachable code detected
server.Stop();
#pragma warning restore CS0162 // Unreachable code detected

static async Task NugetRespondToRequest(HttpListenerResponse response, NameValueCollection args, ILogger logger, NugetGetDownloadsInfo nugetBadgeInfo)
{
    var user = args["user"];

    if (user is null)
    {
        logger.LogWarning("User is missing, skipping");
        return;
    }

    var responseString = await nugetBadgeInfo.GetInfo(user);
    ProcessResponse(responseString, response);
}

static void ProcessResponse(string responseString, HttpListenerResponse response)
{
    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
    response.ContentLength64 = buffer.Length;
    Stream output = response.OutputStream;
    response.Headers.Add("content-type", "application/json");
    output.Write(buffer, 0, buffer.Length);
    output.Close();
}