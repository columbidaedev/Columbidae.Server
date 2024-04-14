using Columbidae.Server.Core.Message;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using CMsg = Columbidae.Message.Message;

namespace Columbidae.Server.Core.Service.Impl;

public class FcmBroadcast : IBroadcast
{
    public bool IsAvailable() => true;

    public int GetPriority() => 0;

    private readonly FirebaseMessaging _fcm;

    public FcmBroadcast(string projectId, GoogleCredential? credential = null)
    {
        var app = FirebaseApp.Create(new AppOptions
        {
            Credential = credential ?? GoogleCredential.GetApplicationDefault(),
            ProjectId = projectId
        });
        _fcm = FirebaseMessaging.GetMessaging(app);
    }

    public async Task OnMessage(CMsg msg, DateTime time)
    {
        
    }
}