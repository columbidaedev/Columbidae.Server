using Columbidae.Message;
using Grpc.Core;

namespace Columbidae.Server.Core.Message;

public class ImService(ColumbidaeContext columbidaeContext) : InstantMessage.InstantMessageBase
{
    public override async Task<SendMessageReply> SendMessage(Columbidae.Message.Message request,
        ServerCallContext context)
    {
        return await base.SendMessage(request, context);
    }

    public override async Task CreateStream(Token request,
        IServerStreamWriter<Columbidae.Message.Message> responseStream, ServerCallContext context)
    {
        await base.CreateStream(request, responseStream, context);
    }

    public override async Task CreateResourceStream(ResourceStreamParam request,
        IServerStreamWriter<Chunk> responseStream, ServerCallContext context)
    {
        await base.CreateResourceStream(request, responseStream, context);
    }

    public override async Task CreateHistoryStream(HistoryStreamParam request,
        IServerStreamWriter<HistoryBlock> responseStream, ServerCallContext context)
    {
        await base.CreateHistoryStream(request, responseStream, context);
    }
}