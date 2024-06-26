using System.Diagnostics;
using System.Globalization;
using System.Threading.Channels;
using Columbidae.Server.Core.Message;
using Columbidae.Server.Core.PersistentStorage;
using Columbidae.Server.Core.Registry;
using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Common.Interface.Api;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server.Core.Bot;

public class LagrangeBot : IBot
{
    public bool Online { get; private set; }
    public ChannelReader<string> LoginUrl => _loginUrlChan.Reader;
    public ColumbidaeContext? Context { get; set; }

    private readonly Logging _logging = new("LagrangeBot");
    private readonly BotContext _bot;
    private readonly ILibrary _library;
    private readonly string _cacheRoot;

    private readonly Channel<string> _loginUrlChan = Channel.CreateBounded<string>(new BoundedChannelOptions(1)
    {
        SingleWriter = true,
        SingleReader = false,
    });

    public LagrangeBot(string cacheRoot, ILibrary library)
    {
        Enum.TryParse(library.Preferences.Value.Bot.Protocol, true, out Protocols botProtocol);
        _library = library;
        _bot = BotFactory.Create(
            new BotConfig
            {
                Protocol = botProtocol,
                CustomSignProvider = new LinuxSigner(library.Preferences.Value.SignServer)
            },
            library.Preferences.Value.Device.ToDeviceInfo(),
            library.Keystore.Value
        );
        _cacheRoot = cacheRoot;
    }

    public async Task Login()
    {
        if (Online)
        {
            return;
        }

        _bot.Invoker.OnBotOnlineEvent += async (_, ev) =>
        {
            Online = true;
            _logging.Delegated.LogInformation("Bot online at {login time}",
                ev.EventTime.ToString(CultureInfo.CurrentCulture));

            _library.Keystore.Value = _bot.UpdateKeystore();
            await _library.Keystore.Write();
            _logging.Delegated.LogDebug("Bot updated keystore");
        };

        _bot.Invoker.OnFriendMessageReceived += async (_, @event) =>
        {
            Context.MessageStorages.SaveMessage(@event.Chain.ToCMsg());
            await Task.WhenAll(
                Context.Broadcasts.GetAll().Select(b => b.OnMessage(@event.Chain.ToCMsg(), @event.EventTime)));
        };

        if (_library.Preferences.Value.ShowBotLog)
        {
            _bot.Invoker.OnBotLogEvent += (_, @event) =>
            {
                _logging.Delegated.Log(logLevel: @event.Level.ToMsLevel(), eventId: new EventId(),
                    message: @event.EventMessage);
            };
        }

        var account = _library.Preferences.Value.Account;
        if (account != null && _library.Keystore.Value.Uin == account.Uin)
        {
            var succeeded = await _bot.LoginByPassword();
            if (!succeeded)
            {
                await LoginViaQr();
            }
        }
        else
        {
            await LoginViaQr();
        }
    }

    private async Task LoginViaQr()
    {
        _logging.Delegated.LogInformation("Fetching QR code");
        var login = await _bot.FetchQrCode();
        if (!login.HasValue)
        {
            _logging.Delegated.LogError("Failed to load login QR code");
        }
        else
        {
            var qrCodeFile = Path.Combine(_cacheRoot, "login_qr_code.png");
            if (_library.Preferences.Value.ShowBotLog)
            {
                await File.WriteAllBytesAsync(qrCodeFile, login.Value.QrCode);
                _logging.Delegated.LogDebug("Login QR code saved to {qr code filepath}", qrCodeFile);
                _logging.Delegated.LogInformation("Waiting for logging in...");
            }

            Process.Start(new ProcessStartInfo(qrCodeFile)
            {
                UseShellExecute = true
            });

            await _loginUrlChan.Writer.WriteAsync(login.Value.Url);
            await _bot.LoginByQrCode();
            _logging.Delegated.LogDebug("Scan returned");
        }
    }

    public Task Shutdown()
    {
        _bot.Dispose();
        return Task.CompletedTask;
    }
}