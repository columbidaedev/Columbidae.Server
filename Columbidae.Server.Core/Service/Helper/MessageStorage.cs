using Columbidae.Server.Core.Registry;
using CMsg = Columbidae.Message.Message;

namespace Columbidae.Server.Core.Service.Helper;

public static class MessageStorage
{
    public static async Task SaveMessage(this Registry<IMessageStorage> context, CMsg message)
    {
        var prioritized = context.GetPrior();
        if (prioritized == null) throw new NullReferenceException("No available registry");
        await prioritized.SaveMessage(message);
    }
}