using System.Threading.Channels;
using Columbidae.Message;

namespace Columbidae.Server.Core.Bot;


public interface IBot
{
    public bool Online { get; }
    public ChannelReader<string> LoginUrl { get; }
    public Task SendMessage(MessageCreator creator);
    public Task Initialize();
    public Task Shutdown();
}