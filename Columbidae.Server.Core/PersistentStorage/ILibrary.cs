using Columbidae.Server.Core.PersistentStorage.Models;
using Lagrange.Core.Common;

namespace Columbidae.Server.Core.PersistentStorage;

public interface ILibrary
{
    public Proxy<PreferencesModel> Preferences { get; set; }
    public Proxy<BotKeystore> Keystore { get; set; }
    public Proxy<ServicesModel> Services { get; set; }
}