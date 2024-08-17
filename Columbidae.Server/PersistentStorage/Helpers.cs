using Columbidae.Server.Core.PersistentStorage;

namespace Columbidae.Server.PersistentStorage;

public class InherentReadWrite<T, TK>(IReadWrite<TK> delegated, Func<TK, T> getter, Func<T, TK> setter) : IReadWrite<T>
{
    public T Read()
    {
        return getter(delegated.Read());
    }

    public async Task Write(T value)
    {
        await delegated.Write(setter(value));
    }
}

public class MemoryReadIoWriteDelegate<T>(ReadWriteDelegate<T> @delegate) : IReadWrite<T>
{
    public T Read()
    {
        return @delegate.Value;
    }

    public async Task Write(T value)
    {
        await @delegate.Write(value);
    }
}