using Columbidae.Server.Core.PersistentStorage.Models;
using Columbidae.Server.Core.Preferences.Models;

namespace Columbidae.Server.Core.Preferences;

public interface ILibrary
{
    public ReadWriteDelegate<PreferencesModel> Preferences { get; set; }
    public ReadWriteDelegate<ServicesModel> Services { get; set; }
}