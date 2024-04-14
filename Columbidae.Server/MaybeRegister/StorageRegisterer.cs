using Columbidae.Server.Core;
using Columbidae.Server.Core.Message;
using Columbidae.Server.Core.PersistentStorage;
using Columbidae.Server.Core.PersistentStorage.Models;
using Columbidae.Server.Core.Registry;
using Columbidae.Server.Core.Service;
using Columbidae.Server.Core.Service.Impl;

namespace Columbidae.Server.MaybeRegister;

public static class StorageRegisterer
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

    private static IAuthenticationStorage? _parse(DatabaseModel model)
    {
        if (model.SqlitePath != null)
        {
            return new SqliteAuthStorage(model.SqlitePath);
        }

        return null;
    }

    private static IAuthenticationStorage _useDefault(IContainer container)
    {
        return new SqliteAuthStorage(Path.Combine(container.DataRoot, "authentication.db"));
    }
}