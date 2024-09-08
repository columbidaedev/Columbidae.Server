using Columbidae.Auth;
using Columbidae.Server.Core.Service;

namespace Columbidae.Server.Core.Registry;

public static class Authentication
{
    public static async Task<bool> HasAuthorizedDevice(this Registry<IAuthenticationStorage> registry, string token)
    {
        return (await Task.WhenAll(registry.GetAvailable()
            .Select(s => s.HasToken(token))
            .ToArray())).Any(registered => registered);
    }

    public static async Task AddAuthorizedDevice(this Registry<IAuthenticationStorage> registry, string token,
        DeviceInfo device)
    {
        var storage = registry.GetPrior();
        if (storage == null) throw new NullReferenceException("null authentication storage");

        await storage.AddDevice(token, device);
    }
}