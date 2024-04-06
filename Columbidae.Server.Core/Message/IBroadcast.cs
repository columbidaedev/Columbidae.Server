using CMsg = Columbidae.Message.Message;

namespace Columbidae.Server.Core.Message;

public interface IBroadcast
{
    Task OnMessage(CMsg msg, DateTime time);
}