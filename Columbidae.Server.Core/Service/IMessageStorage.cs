using Columbidae.Message;
using Columbidae.Server.Core.Registry;
using Grpc.Core;
using CMsg = Columbidae.Message.Message;

namespace Columbidae.Server.Core.Service;

public interface IMessageStorage : IRegisterable
{
    public Task<CMsg?> GetMessage(ulong id);
    public Task<bool> StreamResource(IAsyncStreamWriter<Chunk> writer, ulong frameId, ResourceType type);
    public Task<bool> SaveMessage(CMsg message);
}