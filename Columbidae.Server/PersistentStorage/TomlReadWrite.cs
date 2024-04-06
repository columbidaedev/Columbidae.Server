using Columbidae.Server.Core.PersistentStorage;
using Tomlyn;

namespace Columbidae.Server.PersistentStorage;

public class TomlReadWrite<T> : IReadWrite<T> where T : class, new()
{
    private readonly TomlModelOptions _tomlOptions = new()
    {
        ConvertToModel = (o, type) =>
        {
            if (type == typeof(Guid) && o is string s)
            {
                return Guid.Parse(s);
            }

            return null;
        },
        ConvertToToml = o => o is Guid ? o.ToString() : null
    };


    public T Read(string filepath)
    {
        T value;
        if (File.Exists(filepath))
        {
            using var textStream = File.OpenText(filepath);
            value = Toml.ToModel<T>(textStream.ReadToEnd(), options: _tomlOptions);
        }
        else
        {
            value = new T();
        }

        return value;
    }

    public async Task Write(string filepath, T value)
    {
        await using var file = new StreamWriter(filepath);
        await file.WriteAsync(Toml.FromModel(value, options: _tomlOptions));
        file.Close();
    }
}