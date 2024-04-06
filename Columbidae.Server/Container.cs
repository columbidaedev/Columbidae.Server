using Columbidae.Server.Core.PersistentStorage;
using Directories.Net;

namespace Columbidae.Server;

public class Container : IContainer
{
    public string ConfigurationRoot { get; }
    public string CacheRoot { get; }

    public Container()
    {
        var dirs = ProjectDirectories.From("com", "Columbidae", "Columbidae.Server");
        ConfigurationRoot = dirs.ConfigDir;
        CacheRoot = dirs.CacheDir;
        Directory.CreateDirectory(ConfigurationRoot);
        Directory.CreateDirectory(CacheRoot);
    }
}