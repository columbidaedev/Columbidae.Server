using Columbidae.Server;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;

var container = new Container();
var library = new Library(container);

Enum.TryParse(library.Preferences.Value.Bot.Protocol, true, out Protocols botProtocol);

var bot = BotFactory.Create(
    new BotConfig
    {
        Protocol = botProtocol,
        CustomSignProvider = new LinuxSigner(library.Preferences.Value.SignServer)
    },
    library.Preferences.Value.Device.ToDeviceInfo(),
    library.Keystore.Value);

var messaged = new MessageDelegate(container, library, bot);
await messaged.Login();