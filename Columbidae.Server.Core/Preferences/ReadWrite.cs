namespace Columbidae.Server.Core.Preferences;

public class ReadWriteDelegate<T> : IReadWrite<T>
{
    public T Value { get; set; }
    private readonly IReadWrite<T> _readWrite;

    public T Read()
    {
        var value = _readWrite.Read();
        Value = value;
        return value;
    }

    public async Task Write()
    {
        await _readWrite.Write(Value);
    }

    public async Task Write(T value)
    {
        Value = value;
        await Write();
    }

    public ReadWriteDelegate(IReadWrite<T> readWrite)
    {
        _readWrite = readWrite;
        Value = Read();
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