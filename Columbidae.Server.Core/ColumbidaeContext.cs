using Columbidae.Server.Core.Bot;
using Columbidae.Server.Core.Registry;
using Columbidae.Server.Core.Service;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server.Core;

public class ColumbidaeContext : IAsyncDisposable
{
    private readonly Logging _logging = new("Context");

    public IBot? Bot { get; set; }

    public Registry<IBroadcast> Broadcasts { get; } = new(new Logging("Broadcast"));
    public Registry<IMessageStorage> MessageStorages { get; } = new(new Logging("MessageStorage"));
    public Registry<IMessageCache> MessageCaches { get; } = new(new Logging("MessageCache"));
    public Registry<IAuthenticationStorage> AuthenticationStorages { get; } = new(new Logging("AuthenticationStorage"));
    public bool BotOnline => Bot?.Online == true;

    public async Task Initialize()
    {
        if (Bot == null)
        {
            return;
        }
        _logging.Delegated.LogInformation("Signing into {bot}", Bot.GetType().Name);
        await Bot.Initialize();
    }

    public async ValueTask DisposeAsync()
    {
        if (Bot == null)
        {
            return;
        }
        await Bot.Shutdown();
    }
}