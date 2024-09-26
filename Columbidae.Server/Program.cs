using Columbidae.Server;
using Columbidae.Server.Core;
using Columbidae.Server.Core.Bot;
using Columbidae.Server.Core.Message;
using Columbidae.Server.MaybeRegister;
using Microsoft.Extensions.Logging;

var container = new Container();
var library = new Library(container);
if (!library.Preferences.Value.Verbose) Logging.LogLevel = LogLevel.Information;

await using var context = new ColumbidaeContext();
var bot = new LagrangeBot(context, container.CacheRoot, library.BotDelegate, library.AccountDelegate);
context.Bot = bot;
context.Broadcasts.MaybeRegister(library.Services.Value.Fcm);
context.AuthenticationStorages.RegisterOrUseSqlite(library.Services.Value.Database, container);

await using var app = context.CreateWebApplication(library.Preferences.Value.Server);
_ = context.Initialize();
await app.RunAsync();