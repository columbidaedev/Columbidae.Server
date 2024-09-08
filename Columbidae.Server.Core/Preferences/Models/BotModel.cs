using Lagrange.Core.Common;

namespace Columbidae.Server.Core.Preferences.Models;

public class BotModel
{
    public string SignServer { get; set; } = "";
    public string Protocol { get; set; } = "Linux";
    public BotKeystore? Keystore { get; set; }
    public DeviceModel Device { get; set; } = new();
}