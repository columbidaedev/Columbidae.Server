using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server.Core;

public class Registry<T>(Logging? logging = null)
{
    private readonly List<T> _registration = [];
    private readonly Logging _logging = logging ?? Logging.Default;

    public void Register(T registration)
    {
        _registration.Add(registration);
        _logging.Delegated.LogDebug("Registered {name}", registration?.GetType().Name);
    }

    public void Clear()
    {
        _registration.Clear();
    }

    public ReadOnlyCollection<T> GetAll() => _registration.AsReadOnly();
}