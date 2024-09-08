using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server.Core.Registry;

public class Registry<T>(Logging? logging = null) where T : IRegisterable
{
    private readonly Logging _logging = logging ?? Logging.Default;
    private readonly List<T> _registration = [];

    public void Register(T registration)
    {
        _registration.Add(registration);
        _logging.Delegated.LogDebug("Registered {name}", registration?.GetType().Name);
    }

    public void Clear()
    {
        _registration.Clear();
    }

    public ImmutableList<T> GetAll()
    {
        return _registration.ToImmutableList();
    }

    public ImmutableList<T> GetAvailable()
    {
        return _registration.Where(r => r.IsAvailable()).ToImmutableList();
    }

    public T? GetPrior()
    {
        return _registration.MaxBy(r => r.GetPriority());
    }
}