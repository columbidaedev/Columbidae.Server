using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Lagrange.Core.Message;

namespace Columbidae.Server.Core.Message;

public class FirebaseCloudMessaging : IBroadcast
{
    private readonly FirebaseMessaging _messaging;
    public FirebaseCloudMessaging(string projectId, GoogleCredential? credential = null)
    {
        var app = FirebaseApp.Create(new AppOptions
        {
            Credential = credential ?? GoogleCredential.GetApplicationDefault(),
            ProjectId = projectId
        });
        _messaging = FirebaseMessaging.GetMessaging(app);
    }

    public async Task OnMessage(MessageChain chain, DateTime time)
    {
        
    }
}