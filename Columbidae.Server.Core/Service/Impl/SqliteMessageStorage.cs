using Columbidae.Message;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using CMsg = Columbidae.Message.Message;

namespace Columbidae.Server.Core.Service.Impl;

public class SqliteMessageStorage(string dbPath) : IMessageStorage, IDisposable
{
    private readonly DbContext _dbContext = new(dbPath);

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public bool IsAvailable()
    {
        return true;
    }

    public int GetPriority()
    {
        return 0;
    }

    public async Task<CMsg?> GetMessage(ulong id)
    {
        MessageDbContext.MessageStore store;
        try
        {
            store = await _dbContext.Messages.Where(ms => ms.Id == id)
                .Include(messageStore => messageStore.Frames)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        return store.ToMessage();
    }

    public async Task StreamResource(IAsyncStreamWriter<Chunk> writer, ulong frameId, ResourceType type)
    {
        throw new NotImplementedException();
    }

    public async Task SaveMessage(CMsg message)
    {
        await _dbContext.Messages.AddAsync(message.ToMessageStore());
    }

    private class DbContext(string dbPath) : MessageDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}