using Columbidae.Message;
using Columbidae.Server.Core.Registry;
using Grpc.Core;
using Lagrange.Core.Message;
using CMsg = Columbidae.Message.Message;

namespace Columbidae.Server.Core.Service;

public interface IMessageStorage : IRegisterable
{
    public Task<CMsg?> GetMessage(ulong id);
    public Task StreamResource(IAsyncStreamWriter<Chunk> writer, ulong frameId, ResourceType type);
    public Task SaveMessage(CMsg message);
}