using System.Runtime.InteropServices;
using Directories.Net;

namespace Columbidae.Server;

public class Container
{
    public string ConfigurationRoot { get; private set; }
    public string CacheRoot { get; private set; }

    public Container()
    {
        var dirs = ProjectDirectories.From("com", "zhufucdev", "Columbidae");
        ConfigurationRoot = dirs.ConfigDir;
        CacheRoot = dirs.CacheDir;
        Directory.CreateDirectory(ConfigurationRoot);
        Directory.CreateDirectory(CacheRoot);
    }
}