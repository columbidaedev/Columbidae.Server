using Columbidae.Server.Core.PersistentStorage.Models;
using Columbidae.Server.Core.Preferences.Models;
using Columbidae.Server.Core.Registry;
using Columbidae.Server.Core.Service;
using Columbidae.Server.Core.Service.Impl;
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
            new FcmBroadcast(
                projectId: model.ProjectId,
                credential: model.CredentialFile == null ? null : GoogleCredential.FromFile(model.CredentialFile)
            )
        );
    }
}