using Columbidae.Server.Core.Registry;

namespace Columbidae.Server.Core.Service;

public interface IMessageCache : IRegisterable
{
    /**
     * Get a receiving Stream associated with the given token.
     */
    public Task<Stream> CreateResourceReceiver(string token, string caption, ulong size = 0L);

    /**
     * Get a sending Stream associated with the given token;
     */
    public Task<Stream> CreateResourceSender(string token);

    /**
     * Get the underlying file name of the received. The result is guaranteed of the same name as the creating caption.
     */
    public Task<string> GetResourcePath(string token);
}