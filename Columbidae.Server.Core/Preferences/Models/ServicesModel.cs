using Columbidae.Server.Core.Preferences.Models;

namespace Columbidae.Server.Core.PersistentStorage.Models;

public class ServicesModel
{
    public FcmModel? Fcm { get; set; }
    public DatabaseModel? Database { get; set; }
}