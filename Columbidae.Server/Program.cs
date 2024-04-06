using Columbidae.Server;
using Columbidae.Server.MaybeRegister;
using Columbidae.Server.Core.Message;

var container = new Container();
var library = new Library(container);

var messaged = new Messaging(container, library);
messaged.Broadcasts.MaybeRegister(library.Services.Value.Fcm);

await messaged.Login();