using Columbidae.Server.Core;
using Columbidae.Server.Core.Message;
using Columbidae.Server.Core.PersistentStorage.Models;
using Google.Apis.Auth.OAuth2;

namespace Columbidae.Server.MaybeRegister;

public static class FcmPerhaps
{
    public static void MaybeRegister(this Registry<IBroadcast> registry, FcmModel? model)
    {
        if (model == null)
        {
            return;
        }

        registry.Register(
            new FirebaseCloudMessaging(
                projectId: model.ProjectId,
                credential: model.CredentialFile == null ? null : GoogleCredential.FromFile(model.CredentialFile)
            )
        );
    }
}