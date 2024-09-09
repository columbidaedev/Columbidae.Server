using System.Threading.Channels;

namespace Columbidae.Server.Core.Bot;

public interface IBot
{
    public bool Online { get; }
    public ChannelReader<string> LoginUrl { get; }
    public Task Initialize(ColumbidaeContext context);
    public Task Shutdown();
}