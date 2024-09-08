using Columbidae.Server;
using Columbidae.Server.Core;
using Columbidae.Server.Core.Bot;
using Columbidae.Server.Core.Message;
using Columbidae.Server.MaybeRegister;
using Microsoft.Extensions.Logging;

var container = new Container();
var library = new Library(container);
if (!library.Preferences.Value.Verbose) Logging.LogLevel = LogLevel.Information;

var bot = new LagrangeBot(container.CacheRoot, library.BotDelegate, library.AccountDelegate);
await using var context = new ColumbidaeContext(bot);
context.Broadcasts.MaybeRegister(library.Services.Value.Fcm);
context.AuthenticationStorages.RegisterOrUseSqlite(library.Services.Value.Database, container);

_ = context.Initialize();
await using var app = context.CreateWebApplication(library.Preferences.Value.Server);
await app.RunAsync();