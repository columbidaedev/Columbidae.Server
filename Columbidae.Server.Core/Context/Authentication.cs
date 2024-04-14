using Columbidae.Auth;

namespace Columbidae.Server.Core.Context;

public static class Authentication
{
    public static async Task<bool> HasAuthorizedDevice(this ColumbidaeContext context, string token)
    {
        return (await Task.WhenAll(context.AuthenticationStorages.GetAvailable()
                           .Select(s => s.HasToken(token))
                           .ToArray())).Any(registered => registered);
    }

    public static async Task AddAuthorizedDevice(this ColumbidaeContext context, string token, DeviceInfo device)
    {
        var storage = context.AuthenticationStorages.GetPrior();
        if (storage == null)
        {
            throw new NullReferenceException("null authentication storage");
        }

        await storage.AddDevice(token, device);
    }
}