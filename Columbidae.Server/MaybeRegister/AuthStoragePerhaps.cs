using Columbidae.Server.Core.PersistentStorage;
using Columbidae.Server.Core.PersistentStorage.Models;
using Columbidae.Server.Core.Preferences;
using Columbidae.Server.Core.Preferences.Models;
using Columbidae.Server.Core.Registry;
using Columbidae.Server.Core.Service;
using Columbidae.Server.Core.Service.Impl;

namespace Columbidae.Server.MaybeRegister;

public static class AuthStoragePerhaps
{
    public static void RegisterOrUseSqlite(this Registry<IAuthenticationStorage> registry, DatabaseModel? model,
        IContainer container)

    {
        if (model == null)
        {
            registry.Register(_useDefault(container));
        }
        else
        {
            var parsed = _parse(model);
            registry.Register(parsed ?? _useDefault(container));
        }
    }

    private static SqliteAuthStorage? _parse(DatabaseModel model)
    {
        return model.SqlitePath != null ? new SqliteAuthStorage(model.SqlitePath) : null;
    }

    private static IAuthenticationStorage _useDefault(IContainer container)
    {
        return new SqliteAuthStorage(Path.Combine(container.DataRoot, "authentication.db"));
    }
}