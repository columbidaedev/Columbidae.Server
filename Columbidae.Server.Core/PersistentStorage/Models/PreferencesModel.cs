namespace Columbidae.Server.Core.PersistentStorage.Models;

public class PreferencesModel
{
    public string SignServer { get; set; } = "";
    public bool ShowBotLog { get; set; } = false;
    public ServerModel? Server { get; set; }
    public DeviceModel Device { get; set; } = new DeviceModel();
    public BotModel Bot { get; set; } = new BotModel();
    public AccountModel? Account { get; set; }
}