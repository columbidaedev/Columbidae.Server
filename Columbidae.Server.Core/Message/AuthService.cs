using Columbidae.Auth;
using Columbidae.Server.Core.Registry;
using Grpc.Core;
using NanoidDotNet;
using Authentication = Columbidae.Auth.Authentication;

namespace Columbidae.Server.Core.Message;

public class AuthService : Authentication.AuthenticationBase
{
    private readonly ColumbidaeContext _context;
    private readonly Dictionary<string, DeviceInfo> _unauthorizedDevices = new();

    public AuthService(ColumbidaeContext context)
    {
        _context = context;
    }

    public override async Task RequestLogin(AuthParams request, IServerStreamWriter<AuthResult> responseStream,
        ServerCallContext context)
    {
        var authorized = await _context.AuthenticationStorages.HasAuthorizedDevice(request.AuthToken);
        var unauthorized = _unauthorizedDevices.ContainsKey(request.AuthToken);
        if (!authorized && !unauthorized)
        {
            await responseStream.WriteAsync(new AuthResult
            {
                Finished = true,
                Forbidden = true
            });
        }
        else if (_context.BotOnline)
        {
            if (authorized)
                await responseStream.WriteAsync(new AuthResult
                {
                    Finished = true,
                    Forbidden = false,
                    Online = true
                });
            // TODO: notify existing devices
        }
        else
        {
            var url = await _context.Bot.LoginUrl.ReadAsync();
            if (!authorized)
            {
                var device = _unauthorizedDevices[request.AuthToken];
                await _context.AuthenticationStorages.AddAuthorizedDevice(request.AuthToken, device);
                _unauthorizedDevices.Remove(request.AuthToken);
            }

            await responseStream.WriteAsync(new AuthResult
            {
                Finished = false,
                Forbidden = false,
                Online = false,
                QqAuthUrl = url
            });
        }
    }

    public override async Task<GetTokenResult> GetToken(DeviceInfo request, ServerCallContext context)
    {
        var token = await Nanoid.GenerateAsync();
        if (token == null) throw new NullReferenceException("nanoid gen");

        _unauthorizedDevices[token] = request;
        return new GetTokenResult
        {
            AuthToken = token
        };
    }
}