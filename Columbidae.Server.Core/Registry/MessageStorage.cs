using Columbidae.Server.Core.Service;
using CMsg = Columbidae.Message.Message;

namespace Columbidae.Server.Core.Registry;

public static class MessageStorage
{
    public static void SaveMessage(this Registry<IMessageStorage> context, CMsg message)
    {
        var prioritized = context.GetPrior();
        if (prioritized == null) throw new NullReferenceException("No available registry");
        prioritized.SaveMessage(message);
    }
}