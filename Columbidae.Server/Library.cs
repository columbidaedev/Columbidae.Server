using Columbidae.Server.Core.PersistentStorage;
using Columbidae.Server.Core.PersistentStorage.Models;
using Columbidae.Server.PersistentStorage;

namespace Columbidae.Server;

public class Library : ILibrary
{
    public ReadWriteDelegate<PreferencesModel> Preferences { get; set; }
    public ReadWriteDelegate<ServicesModel> Services { get; set; }

    public ReadWriteDelegate<BotModel> BotDelegate
    {
        get
        {
            var deg = new InherentReadWrite<BotModel, PreferencesModel>(
                delegated: new MemoryReadIoWriteDelegate<PreferencesModel>(Preferences),
                getter: pref => pref.Bot,
                setter: bot =>
                {
                    Preferences.Value.Bot = bot;
                    return Preferences.Value;
                }
            );
            return new ReadWriteDelegate<BotModel>(deg);
        }
    }


    public ReadWriteDelegate<AccountModel?> AccountDelegate
    {
        get
        {
            var deg = new InherentReadWrite<AccountModel?, PreferencesModel>(
                delegated: new MemoryReadIoWriteDelegate<PreferencesModel>(Preferences),
                getter: pref => pref.Account,
                setter: account =>
                {
                    Preferences.Value.Account = account;
                    return Preferences.Value;
                }
            );
            return new ReadWriteDelegate<AccountModel?>(deg);
        }
    }

    public Library(IContainer container)
    {
        var prefFile = Path.Combine(container.ConfigurationRoot, "preferences.toml");
        var servicesFile = Path.Combine(container.ConfigurationRoot, "services.toml");
        Preferences = new ReadWriteDelegate<PreferencesModel>(new TomlReadWrite<PreferencesModel>(prefFile));
        Services = new ReadWriteDelegate<ServicesModel>(new TomlReadWrite<ServicesModel>(servicesFile));
    }
}