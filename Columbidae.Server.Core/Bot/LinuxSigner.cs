using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using Lagrange.Core.Utility.Sign;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Columbidae.Server.Core.Bot;

internal class LinuxSigner : SignProvider
{
    private readonly HttpClient _client = new();


    private readonly Timer _timer;
    private readonly string _url;

    public LinuxSigner(string signServer)
    {
        _timer = new Timer(_ =>
        {
            var reconnect = Available = Test();
            if (reconnect) _timer?.Change(-1, 5000);
        });
        _url = signServer;
    }

    public override byte[]? Sign(string cmd, uint seq, byte[] body, out byte[]? ver, out string? token)
    {
        ver = null;
        token = null;
        if (!WhiteListCommand.Contains(cmd)) return null;
        if (!Available || string.IsNullOrEmpty(_url)) return new byte[35]; // Dummy signature

        var payload = new JsonObject
        {
            { "cmd", cmd },
            { "seq", seq },
            { "src", body.Hex() }
        };

        try
        {
            var message = _client.PostAsJsonAsync(_url, payload).Result;
            var response = message.Content.ReadAsStringAsync().Result;
            var json = JObject.Parse(response);

            ver = json["value"]?["extra"]?.ToString().UnHex() ?? Array.Empty<byte>();
            token = Encoding.ASCII.GetString(json?["value"]?["token"]?.ToString().UnHex() ?? Array.Empty<byte>());
            return json?["value"]?["sign"]?.ToString().UnHex() ?? new byte[20];
        }
        catch (Exception)
        {
            Available = false;
            _timer.Change(0, 5000);

            Logging.Logger.LogCritical("Failed to get signature, using dummy signature");
            return new byte[20]; // Dummy signature
        }
    }

    public override bool Test()
    {
        try
        {
            var response = _client.GetAsync($"{_url}/ping").GetAwaiter().GetResult().Content.ReadAsStringAsync()
                .GetAwaiter().GetResult();
            if (JObject.Parse(response)["code"]?.Value<int>() == 0) return true;
        }
        catch
        {
            return false;
        }

        return false;
    }
}