using System.Net;
using Columbidae.Server.Core.PersistentStorage.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server.Core.Message;

public static class CreateWebApp
{
    public static WebApplication CreateWebApplication(this ColumbidaeContext context, ServerModel server)
    {
        var listen = server.Listen?.Split(":") ?? [];

        var appBuilder = WebApplication.CreateBuilder();
        appBuilder.Logging.SetMinimumLevel(Logging.LogLevel);
        appBuilder.Services.AddGrpc();
        appBuilder.Services.AddSingleton(context);
        appBuilder.WebHost.ConfigureKestrel(options =>
        {
            switch (listen.Length)
            {
                case <= 0:
                    options.Listen(IPAddress.Any, 5950, ConfigureListen);
                    break;
                case 1:
                    options.Listen(IPAddress.Parse(listen[0]), 5950, ConfigureListen);
                    break;
                case 2:
                    options.Listen(IPAddress.Parse(listen[0]), int.Parse(listen[1]), ConfigureListen);
                    break;
                default:
                    throw new FormatException($"Unknown listen address: {server.Listen}");
            }
        });

        var app = appBuilder.Build();
        app.MapGrpcService<ImService>();
        app.MapGrpcService<AuthService>();

        return app;

        void ConfigureListen(ListenOptions listenOptions)
        {
            listenOptions.Protocols = HttpProtocols.Http2;
            if (server.CertificatePath != null)
            {
                listenOptions.UseHttps(server.CertificatePath, server.CertificatePassword);
            }
            else
            {
                listenOptions.UseHttps();
            }
        }
    }
}