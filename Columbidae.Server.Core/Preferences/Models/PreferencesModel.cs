using Columbidae.Server.Core.PersistentStorage.Models;

namespace Columbidae.Server.Core.Preferences.Models;

public class PreferencesModel
{
    public bool Verbose { get; set; } = false;
    public ServerModel? Server { get; set; }
    public BotModel Bot { get; set; } = new BotModel();
    public AccountModel Account { get; set; }
}