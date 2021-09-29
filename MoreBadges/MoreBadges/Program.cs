// See https://aka.ms/new-console-template for more information
using NuGet.Common;
using System.Net;

using var server = new HttpListener();

var a = new List<int>();

// netsh http add urlacl url=http://+:11456/ user=DESKTOP-1IF9LEB\goose
// server.Prefixes.Add("http://+:11456/");
server.Prefixes.Add("http://+:11456/");

IWriterReader writerReader = new DebugWriterReader("./caches");
ILogger logger = new DebugLogger();

var nugetBadgeInfo = new NugetGetDownloadsInfo(writerReader, logger);

server.Start();

while (true)
{
    var context = server.GetContext();
    var request = context.Request;
    var response = context.Response;
    var user = request.QueryString["user"];

    if (user is null)
    {
        logger.LogWarning("User is missing, skipping");
        continue;
    }

    var responseString = nugetBadgeInfo.GetInfo(user);

    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
    response.ContentLength64 = buffer.Length;
    System.IO.Stream output = response.OutputStream;
    response.Headers.Add("content-type", "application/json");
    output.Write(buffer, 0, buffer.Length);
    output.Close();
}
// response.ContentType = "image/svg+xml";
// You must close the output stream.

server.Stop();