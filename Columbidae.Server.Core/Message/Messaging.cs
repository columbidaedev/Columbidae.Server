using System.Diagnostics;
using System.Globalization;
using Columbidae.Server.Core.Bot;
using Columbidae.Server.Core.PersistentStorage;
using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Common.Interface.Api;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server.Core.Message;

public class Messaging
{
    private readonly Logging _logging = new("Message Delegate");
    public Registry<IBroadcast> Broadcasts { get; } = new(new Logging("Broadcast"));
    private readonly BotContext _bot;
    private readonly ILibrary _library;
    private readonly IContainer _container;

    public Messaging(IContainer container, ILibrary library)
    {
        _library = library;
        _container = container;

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
        _bot.Invoker.OnBotOnlineEvent += async (_, ev) =>
        {
            _logging.Delegated.LogInformation("Bot online at {login time}",
                ev.EventTime.ToString(CultureInfo.CurrentCulture));

            _library.Keystore.Value = _bot.UpdateKeystore();
            await _library.Keystore.Write();
            _logging.Delegated.LogDebug("Bot updated keystore");
        };

        _bot.Invoker.OnFriendMessageReceived += async (_, @event) =>
        {
            await Task.WhenAll(Broadcasts.GetAll().Select(b => b.OnMessage(@event.Chain.ToCMsg(), @event.EventTime)));
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
        var qrCode = (await _bot.FetchQrCode())?.QrCode;
        if (qrCode == null)
        {
            Logging.Logger.LogError("Failed to load login QR code");
        }
        else
        {
            var qrCodeFile = Path.Combine(_container.CacheRoot, "login_qr_code.png");
            await File.WriteAllBytesAsync(qrCodeFile, qrCode);
            Logging.Logger.LogDebug("Login QR code saved to {qr code filepath}", qrCodeFile);
            Logging.Logger.LogInformation("Waiting for logging in...");
            Process.Start(new ProcessStartInfo(qrCodeFile)
            {
                UseShellExecute = true
            });
            await _bot.LoginByQrCode();
            Logging.Logger.LogDebug("Scan returned");
        }
    }
}