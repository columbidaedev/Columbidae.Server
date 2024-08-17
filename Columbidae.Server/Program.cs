using Columbidae.Server;
using Columbidae.Server.Core;
using Columbidae.Server.Core.Bot;
using Columbidae.Server.MaybeRegister;
using Microsoft.Extensions.Logging;

var container = new Container();
var library = new Library(container);
if (!library.Preferences.Value.Verbose)
{
    Logging.LogLevel = LogLevel.Critical;
}

var bot = new LagrangeBot(container.CacheRoot, library.BotDelegate, library.AccountDelegate);
var messaged = new ColumbidaeContext(library.Preferences.Value.Server, bot);
messaged.Broadcasts.MaybeRegister(library.Services.Value.Fcm);
messaged.AuthenticationStorages.RegisterOrUseSqlite(library.Services.Value.Database, container);

_ = messaged.Login();
await messaged.Run();

await messaged.Shutdown();