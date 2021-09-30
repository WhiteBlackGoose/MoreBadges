﻿using NuGet.Common;
using System.Collections.Specialized;
using System.Net;

using var server = new HttpListener();

server.Prefixes.Add("http://+:11456/");

ILogger logger = new DebugLogger();
IWriterReader writerReader = new DebugWriterReader("./caches", logger);

var runningTasks = new RunningTasksCollection();

var nugetBadgeInfo = new NugetGetDownloadsInfo(writerReader, logger);

server.Start();

while (true)
{
    var context = server.GetContext();
    var request = context.Request;
    var response = context.Response;
    
    runningTasks.Add(NugetRespondToRequest(response, request.QueryString, logger, nugetBadgeInfo));

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