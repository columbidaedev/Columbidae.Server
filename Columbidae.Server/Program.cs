using Columbidae.Server;
using Columbidae.Server.Core;
using Columbidae.Server.Core.Bot;
using Columbidae.Server.MaybeRegister;

var container = new Container();
var library = new Library(container);

var bot = new LagrangeBot(container.CacheRoot, library.BotDelegate, library.AccountDelegate);
var messaged = new ColumbidaeContext(library.Preferences.Value.Server, bot);
messaged.Broadcasts.MaybeRegister(library.Services.Value.Fcm);
messaged.AuthenticationStorages.RegisterOrUseSqlite(library.Services.Value.Database, container);

_ = messaged.Login();
await messaged.Run();

await messaged.Shutdown();