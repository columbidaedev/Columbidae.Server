using Columbidae.Server.Core.Registry;
using CMsg = Columbidae.Message.Message;

namespace Columbidae.Server.Core.Service;

public interface IBroadcast : IRegisterable
{
    Task OnMessage(CMsg msg, DateTime time);
}