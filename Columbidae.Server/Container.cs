using Columbidae.Server.Core.PersistentStorage;
using Columbidae.Server.Core.Preferences;
using Directories.Net;

namespace Columbidae.Server;

public class Container : IContainer
{
    public string ConfigurationRoot { get; }
    public string CacheRoot { get; }
    public string DataRoot { get; }

    public Container()
    {
        var dirs = ProjectDirectories.From("com", "Columbidae", "Columbidae.Server");
        ConfigurationRoot = dirs.ConfigDir;
        CacheRoot = dirs.CacheDir;
        DataRoot = dirs.DataDir;
        Directory.CreateDirectory(ConfigurationRoot);
        Directory.CreateDirectory(CacheRoot);
    }
}