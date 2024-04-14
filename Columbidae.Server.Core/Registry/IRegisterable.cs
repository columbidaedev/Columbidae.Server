namespace Columbidae.Server.Core.Registry;

public interface IRegisterable
{
    bool IsAvailable();
    int GetPriority();
}