using System.Diagnostics;
using System.Globalization;
using System.Threading.Channels;
using Columbidae.Server.Core.Bot;
using Columbidae.Server.Core.Message;
using Columbidae.Server.Core.PersistentStorage;
using Columbidae.Server.Core.PersistentStorage.Models;
using Columbidae.Server.Core.Registry;
using Columbidae.Server.Core.Service;
using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Common.Interface.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server.Core;

public class ColumbidaeContext
{
    public Registry<IBroadcast> Broadcasts { get; } = new(new Logging("Broadcast"));
    public Registry<IMessageStorage> MessageStorages { get; } = new(new Logging("MessageStorage"));
    public Registry<IAuthenticationStorage> AuthenticationStorages { get; } = new(new Logging("AuthenticationStorage"));
    private readonly Logging _logging = new("Message Delegate");
    private readonly BotContext _bot;
    private readonly ILibrary _library;
    private readonly IContainer _container;
    private readonly WebApplication _app;

    private readonly Channel<string> _loginUrlChan = Channel.CreateBounded<string>(new BoundedChannelOptions(1)
    {
        SingleWriter = true,
        SingleReader = false,
    });

    public bool BotOnline { get; private set; }
    public ChannelReader<string> LoginUrlChannel => _loginUrlChan.Reader;

    public ColumbidaeContext(IContainer container, ILibrary library)
    {
        _library = library;
        _container = container;
        _app = this.CreateWebApplication(library.Preferences.Value.Server ?? new ServerModel());

        Enum.TryParse(library.Preferences.Value.Bot.Protocol, true, out Protocols botProtocol);
        _bot = BotFactory.Create(
            new BotConfig
            {
                Protocol = botProtocol,
                CustomSignProvider = new LinuxSigner(library.Preferences.Value.SignServer)
            },
            library.Preferences.Value.Device.ToDeviceInfo(),
            library.Keystore.Value
        );
    }

    public async Task Login()
    {
        if (BotOnline)
        {
            return;
        }

        _bot.Invoker.OnBotOnlineEvent += async (_, ev) =>
        {
            BotOnline = true;
            _logging.Delegated.LogInformation("Bot online at {login time}",
                ev.EventTime.ToString(CultureInfo.CurrentCulture));

            _library.Keystore.Value = _bot.UpdateKeystore();
            await _library.Keystore.Write();
            _logging.Delegated.LogDebug("Bot updated keystore");
        };

        _bot.Invoker.OnFriendMessageReceived += async (_, @event) =>
        {
            MessageStorages.SaveMessage(@event.Chain.ToCMsg());
            await Task.WhenAll(
                Broadcasts.GetAll().Select(b => b.OnMessage(@event.Chain.ToCMsg(), @event.EventTime)));
        };

        if (_library.Preferences.Value.ShowBotLog)
        {
            var botLogging = new Logging("Lagrange");
            _bot.Invoker.OnBotLogEvent += (_, @event) =>
            {
                botLogging.Delegated.Log(logLevel: @event.Level.ToMsLevel(), eventId: new EventId(),
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
        var login = await _bot.FetchQrCode();
        if (!login.HasValue)
        {
            Logging.Logger.LogError("Failed to load login QR code");
        }
        else
        {
            if (_library.Preferences.Value.ShowBotLog)
            {
                var qrCodeFile = Path.Combine(_container.CacheRoot, "login_qr_code.png");
                await File.WriteAllBytesAsync(qrCodeFile, login.Value.QrCode);
                Logging.Logger.LogDebug("Login QR code saved to {qr code filepath}", qrCodeFile);
                Logging.Logger.LogInformation("Waiting for logging in...");
                Process.Start(new ProcessStartInfo(qrCodeFile)
                {
                    UseShellExecute = true
                });
            }

            await _loginUrlChan.Writer.WriteAsync(login.Value.Url);
            await _bot.LoginByQrCode();
            Logging.Logger.LogDebug("Scan returned");
        }
    }

    public async Task Run()
    {
        await _app.RunAsync();
    }
}