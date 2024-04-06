using Lagrange.Core.Message;

namespace Columbidae.Server.Core.Message;

public interface IBroadcast
{
    Task OnMessage(MessageChain chain, DateTime time);
}