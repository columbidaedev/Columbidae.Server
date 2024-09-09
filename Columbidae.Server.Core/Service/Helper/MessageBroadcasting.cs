using Columbidae.Server.Core.Registry;

namespace Columbidae.Server.Core.Service.Helper;

public static class MessageBroadcasting
{
    public static async Task BroadcastToAll(this Registry<IBroadcast> registry, Columbidae.Message.Message message,
        DateTime eventTime)
    {
        await Task.WhenAll(registry.GetAll().Select(b => b.OnMessage(message, eventTime)));
    }
}