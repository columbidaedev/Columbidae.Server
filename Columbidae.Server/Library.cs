using Columbidae.Server.Core.PersistentStorage;
using Columbidae.Server.Core.PersistentStorage.Models;
using Columbidae.Server.PersistentStorage;
using Lagrange.Core.Common;

namespace Columbidae.Server;

public class Library : ILibrary
{
    public Proxy<PreferencesModel> Preferences { get; set; }
    public Proxy<BotKeystore> Keystore { get; set; }
    public Proxy<ServicesModel> Services { get; set; }

    public Library(Container container)
    {
        var prefFile = Path.Combine(container.ConfigurationRoot, "preferences.toml");
        var keystoreFile = Path.Combine(container.ConfigurationRoot, "keystore.json");
        var servicesFile = Path.Combine(container.ConfigurationRoot, "services.toml");
        Preferences = new Proxy<PreferencesModel>(prefFile, new TomlReadWrite<PreferencesModel>());
        Keystore = new Proxy<BotKeystore>(keystoreFile, new JsonReadWrite<BotKeystore>());
        Services = new Proxy<ServicesModel>(servicesFile, new TomlReadWrite<ServicesModel>());
    }
}