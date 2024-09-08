namespace Columbidae.Server.Core.Preferences;

public class ReadWriteDelegate<T> : IReadWrite<T>
{
    private readonly IReadWrite<T> _readWrite;

    public ReadWriteDelegate(IReadWrite<T> readWrite)
    {
        _readWrite = readWrite;
        Value = Read();
    }

    public T Value { get; set; }

    public T Read()
    {
        var value = _readWrite.Read();
        Value = value;
        return value;
    }

    public async Task Write(T value)
    {
        Value = value;
        await Write();
    }

    public async Task Write()
    {
        await _readWrite.Write(Value);
    }
}

public interface IFileReadWrite<T> : IReadWrite<T>
{
    public string FilePath { get; }
}

public interface IReadWrite<T>
{
    T Read();
    Task Write(T value);
}