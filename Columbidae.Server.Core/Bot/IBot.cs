using System.Threading.Channels;

namespace Columbidae.Server.Core.Bot;

public interface IBot
{
    public bool Online { get; }
    public ColumbidaeContext? Context { set; }
    public ChannelReader<string> LoginUrl { get; }
    public Task Initialize();
    public Task Shutdown();
}