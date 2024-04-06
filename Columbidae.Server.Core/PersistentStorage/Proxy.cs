namespace Columbidae.Server.Core.PersistentStorage;

public class Proxy<T> where T : class, new()
{
    public T Value { get; set; }
    private readonly string _filepath;
    private readonly IReadWrite<T> _readWrite;

    public T Read()
    {
        var value = _readWrite.Read(_filepath);
        Value = value;
        return value;
    }

    public async Task Write()
    {
        await _readWrite.Write(_filepath, Value);
    }

    public Proxy(string filepath, IReadWrite<T> readWrite)
    {
        _filepath = filepath;
        _readWrite = readWrite;
        Value = Read();
    }
}

public interface IReadWrite<T> where T : class, new()
{
    T Read(string filepath);
    Task Write(string filepath, T value);
}