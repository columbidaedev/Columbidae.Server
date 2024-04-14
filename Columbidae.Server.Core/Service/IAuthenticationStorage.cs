using Columbidae.Auth;
using Columbidae.Server.Core.Registry;

namespace Columbidae.Server.Core.Service;

public interface IAuthenticationStorage : IRegisterable
{
    Task AddDevice(string token, DeviceInfo device);
    Task<bool> HasToken(string token);
    Task<DeviceInfo?> GetDevice(string token);
    Task RemoveDevice(string token);
}