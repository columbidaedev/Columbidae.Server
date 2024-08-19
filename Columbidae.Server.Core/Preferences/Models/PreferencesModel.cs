namespace Columbidae.Server.Core.PersistentStorage.Models;

public class PreferencesModel
{
    public bool Verbose { get; set; } = false;
    public ServerModel? Server { get; set; }
    public BotModel Bot { get; set; } = new BotModel();
    public AccountModel Account { get; set; }
}