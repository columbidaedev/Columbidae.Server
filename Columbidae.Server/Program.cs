using Columbidae.Server;
using Columbidae.Server.Core;
using Columbidae.Server.MaybeRegister;
using Columbidae.Server.Core.Message;

var container = new Container();
var library = new Library(container);

var messaged = new ColumbidaeContext(container, library);
messaged.Broadcasts.MaybeRegister(library.Services.Value.Fcm);
messaged.AuthenticationStorages.RegisterOrUseSqlite(library.Services.Value.Database, container);

_ = messaged.Login();
await messaged.Run();
