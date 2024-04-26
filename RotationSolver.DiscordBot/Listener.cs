using System.Net;

namespace RotationSolver.DiscordBot;
internal class Listener : IDisposable
{
    private readonly HttpListener _listener;
    private readonly Action<string> _actStr;
    public Listener(int port, Action<string> actStr)
    {
        _actStr = actStr;
        _listener = new();
        _listener.Prefixes.Add($"http://*:{port}/");
        _listener.Start();
        Receive();
    }

    private void Receive()
    {
        _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
    }

    private void ListenerCallback(IAsyncResult result)
    {
        if (!_listener.IsListening) return;

        var context = _listener.EndGetContext(result);
        var request = context.Request;

        if (request.HasEntityBody)
        {
            Console.WriteLine(request.Url);
            using var body = request.InputStream;
            using var reader = new StreamReader(body, request.ContentEncoding);

            try
            {
                _actStr(reader.ReadToEnd());
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }

        using (var response = context.Response)
        {
            response.StatusCode = 200;
        }

        Receive();
    }

    public void Dispose()
    {
        _listener.Stop();
    }
}
