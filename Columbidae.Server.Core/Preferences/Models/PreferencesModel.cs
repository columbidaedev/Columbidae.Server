namespace Columbidae.Server.Core.Preferences.Models;

public class PreferencesModel
{
    public bool Verbose { get; set; } = false;
    public ServerModel Server { get; set; } = new ServerModel();
    public BotModel Bot { get; set; } = new();
    public AccountModel Account { get; set; }
}