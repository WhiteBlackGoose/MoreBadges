using System.Net;

public static class Server
{
    public static void Respond(string responseString, HttpListenerResponse response)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        Stream output = response.OutputStream;
        response.Headers.Add("content-type", "application/json");
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }
}