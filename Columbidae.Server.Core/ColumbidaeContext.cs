using Columbidae.Server.Core.Bot;
using Columbidae.Server.Core.Message;
using Columbidae.Server.Core.Preferences.Models;
using Columbidae.Server.Core.Registry;
using Columbidae.Server.Core.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server.Core;

public class ColumbidaeContext
{
    private readonly WebApplication _app;
    private readonly Logging _logging = new("Context");

    public readonly IBot Bot;

    public ColumbidaeContext(ServerModel? server, IBot bot)
    {
        Bot = bot;
        Bot.Context = this;
        _app = this.CreateWebApplication(server ?? new ServerModel());
    }

    public Registry<IBroadcast> Broadcasts { get; } = new(new Logging("Broadcast"));
    public Registry<IMessageStorage> MessageStorages { get; } = new(new Logging("MessageStorage"));
    public Registry<IAuthenticationStorage> AuthenticationStorages { get; } = new(new Logging("AuthenticationStorage"));
    public bool BotOnline => Bot.Online;

    public async Task Login()
    {
        _logging.Delegated.LogInformation("Signing into {bot}", Bot.GetType().Name);
        await Bot.Initialize();
    }

    public async Task Run()
    {
        await _app.RunAsync();
    }

    public async Task Shutdown()
    {
        await Bot.Shutdown();
    }
}