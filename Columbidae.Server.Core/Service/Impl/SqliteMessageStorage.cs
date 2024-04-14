using Columbidae.Message;
using Grpc.Core;

namespace Columbidae.Server.Core.Service.Impl;

public class SqliteMessageStorage : IMessageStorage
{
    public bool IsAvailable() => true;
    public int GetPriority() => 0;

    public Task<Columbidae.Message.Message?> GetMessage(ulong id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StreamResource(IAsyncStreamWriter<Chunk> writer, ulong frameId, ResourceType type)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveMessage()
    {
        throw new NotImplementedException();
    }
}