namespace Columbidae.Server.Core.PersistentStorage;

public interface IContainer
{
    public string ConfigurationRoot { get; }
    public string CacheRoot { get; }
}