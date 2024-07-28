using System.Net;

namespace RotationSolver.DiscordBot;
internal class Listener : IDisposable
{
    private readonly HttpListener _listener;
    public Listener(int port)
    {
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
            using var body = request.InputStream;
            using var reader = new StreamReader(body, request.ContentEncoding);
            var contentStr = reader.ReadToEnd();
            var path = request.Url?.AbsolutePath ?? string.Empty;

            try
            {
                switch (path)
                {
                    case "/githubPublish":
                        Service.SendGithubPublish(contentStr);
                        break;

                    case "/githubPush":
                        GithubHelper.SendGithubPush(contentStr);
                        break;

                    case "/kofi":
                        Service.SendKofi(contentStr);
                        break;

                    case "/patreon":
                        Service.SendPatreon(contentStr);

                        var dev = Service.Client.GetChannelAsync(Config.ModeratorChannel).Result;

                        if (dev == null) break;
                        dev.SendMessageAsync(contentStr);
                        break;
                }
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
