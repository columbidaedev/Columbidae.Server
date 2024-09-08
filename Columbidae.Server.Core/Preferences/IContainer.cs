namespace Columbidae.Server.Core.Preferences;

public interface IContainer
{
    public string ConfigurationRoot { get; }
    public string CacheRoot { get; }
    public string DataRoot { get; }
}