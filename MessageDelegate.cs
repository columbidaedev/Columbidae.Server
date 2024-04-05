using System.Diagnostics;
using System.Globalization;
using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server;

public class MessageDelegate(Container container, Library library, BotContext bot)
{
    public async Task Login()
    {
        bot.Invoker.OnBotOnlineEvent += (b, ev) =>
        {
            Logging.Logger.LogInformation("Online at {login time}",
                ev.EventTime.ToString(CultureInfo.CurrentCulture));

            library.Keystore.Value = bot.UpdateKeystore();
            library.Keystore.Write().GetAwaiter().GetResult();
            Logging.Logger.LogDebug("Keystore updated");
        };

        if (library.Preferences.Value.ShowBotLog)
        {
            var botLogging = new Logging("Lagrange");
            bot.Invoker.OnBotLogEvent += (context, ev) =>
            {
                botLogging.Delegated.Log(logLevel: ev.Level.ToMsLevel(), eventId: new EventId(),
                    message: ev.EventMessage);
            };
        }


        var account = library.Preferences.Value.Account;
        if (account != null && library.Keystore.Value.Uin == account.Uin)
        {
            var succeeded = await bot.LoginByPassword();
            if (!succeeded)
            {
                await LoginViaQr();
            }
        }
        else
        {
            await LoginViaQr();
        }

        await library.Preferences.Write();
    }

    private async Task LoginViaQr()
    {
        var qrCode = (await bot.FetchQrCode())?.QrCode;
        if (qrCode == null)
        {
            Logging.Logger.LogError("Failed to load login QR code");
        }
        else
        {
            var qrCodeFile = Path.Combine(container.CacheRoot, "login_qr_code.png");
            await File.WriteAllBytesAsync(qrCodeFile, qrCode);
            Logging.Logger.LogDebug("Login QR code saved to {qr code filepath}", qrCodeFile);
            Logging.Logger.LogInformation("Waiting for logging in...");
            Process.Start(new ProcessStartInfo(qrCodeFile)
            {
                UseShellExecute = true
            });
            await bot.LoginByQrCode();
            Logging.Logger.LogDebug("Scan returned");
        }
    }
}