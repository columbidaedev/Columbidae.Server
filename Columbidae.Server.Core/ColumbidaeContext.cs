using Columbidae.Server.Core.Bot;
using Columbidae.Server.Core.Registry;
using Columbidae.Server.Core.Service;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server.Core;

public class ColumbidaeContext : IAsyncDisposable
{
    private readonly Logging _logging = new("Context");

    public readonly IBot Bot;

    public ColumbidaeContext(IBot bot)
    {
        Bot = bot;
    }

    public Registry<IBroadcast> Broadcasts { get; } = new(new Logging("Broadcast"));
    public Registry<IMessageStorage> MessageStorages { get; } = new(new Logging("MessageStorage"));
    public Registry<IAuthenticationStorage> AuthenticationStorages { get; } = new(new Logging("AuthenticationStorage"));
    public bool BotOnline => Bot.Online;

    public async Task Initialize()
    {
        _logging.Delegated.LogInformation("Signing into {bot}", Bot.GetType().Name);
        await Bot.Initialize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await Bot.Shutdown();
    }
}