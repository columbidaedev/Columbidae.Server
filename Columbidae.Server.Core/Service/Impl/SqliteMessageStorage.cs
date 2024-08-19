using Columbidae.Message;
using Grpc.Core;
using CMsg = Columbidae.Message.Message;

namespace Columbidae.Server.Core.Service.Impl;

public class SqliteMessageStorage(string dbPath) : IMessageStorage
{
    public bool IsAvailable() => true;
    public int GetPriority() => 0;

    public Task<CMsg?> GetMessage(ulong id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StreamResource(IAsyncStreamWriter<Chunk> writer, ulong frameId, ResourceType type)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveMessage(CMsg message)
    {
        throw new NotImplementedException();
    }
}