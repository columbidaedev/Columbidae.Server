using Columbidae.Server.Core.PersistentStorage.Models;
using Columbidae.Server.Core.Preferences;
using Columbidae.Server.Core.Preferences.Models;
using Columbidae.Server.Preferences;

namespace Columbidae.Server;

public class Library : ILibrary
{
    public Library(IContainer container)
    {
        var prefFile = Path.Combine(container.ConfigurationRoot, "preferences.toml");
        var servicesFile = Path.Combine(container.ConfigurationRoot, "services.toml");
        Preferences = new ReadWriteDelegate<PreferencesModel>(new TomlReadWrite<PreferencesModel>(prefFile));
        Services = new ReadWriteDelegate<ServicesModel>(new TomlReadWrite<ServicesModel>(servicesFile));
    }

    public ReadWriteDelegate<BotModel> BotDelegate
    {
        get
        {
            var deg = new InherentReadWrite<BotModel, PreferencesModel>(
                new MemoryReadIoWriteDelegate<PreferencesModel>(Preferences),
                pref => pref.Bot,
                bot =>
                {
                    Preferences.Value.Bot = bot;
                    return Preferences.Value;
                }
            );
            return new ReadWriteDelegate<BotModel>(deg);
        }
    }


    public ReadWriteDelegate<AccountModel> AccountDelegate
    {
        get
        {
            var deg = new InherentReadWrite<AccountModel, PreferencesModel>(
                new MemoryReadIoWriteDelegate<PreferencesModel>(Preferences),
                pref => pref.Account,
                account =>
                {
                    Preferences.Value.Account = account;
                    return Preferences.Value;
                }
            );
            return new ReadWriteDelegate<AccountModel>(deg);
        }
    }

    public ReadWriteDelegate<PreferencesModel> Preferences { get; set; }
    public ReadWriteDelegate<ServicesModel> Services { get; set; }
}