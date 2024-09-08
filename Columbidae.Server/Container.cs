using Columbidae.Server.Core.Preferences;
using Directories.Net;

namespace Columbidae.Server;

public class Container : IContainer
{
    public Container()
    {
        var dirs = ProjectDirectories.From("com", "Columbidae", "Columbidae.Server");
        ConfigurationRoot = dirs.ConfigDir;
        CacheRoot = dirs.CacheDir;
        DataRoot = dirs.DataDir;
        Directory.CreateDirectory(ConfigurationRoot);
        Directory.CreateDirectory(CacheRoot);
    }

    public string ConfigurationRoot { get; }
    public string CacheRoot { get; }
    public string DataRoot { get; }
}