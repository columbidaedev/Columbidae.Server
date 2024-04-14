using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace Columbidae.Server.Core.Registry;

public class Registry<T>(Logging? logging = null) where T : IRegisterable
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

    public ImmutableList<T> GetAll() => _registration.ToImmutableList();

    public ImmutableList<T> GetAvailable() => _registration.Where(r => r.IsAvailable()).ToImmutableList();

    public T? GetPrior() => _registration.MaxBy(r => r.GetPriority());
}