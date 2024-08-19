using Columbidae.Server.Core.PersistentStorage.Models;

namespace Columbidae.Server.Core.PersistentStorage;

public interface ILibrary
{
    public ReadWriteDelegate<PreferencesModel> Preferences { get; set; }
    public ReadWriteDelegate<ServicesModel> Services { get; set; }
}