using Columbidae.Server.Core.PersistentStorage;
using Tomlyn;

namespace Columbidae.Server.PersistentStorage;

public class TomlReadWrite<T>(string filePath) : IFileReadWrite<T>
    where T : class, new()
{
    public string FilePath => filePath;

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


    public T Read()
    {
        T value;
        if (File.Exists(filePath))
        {
            using var textStream = File.OpenText(filePath);
            value = Toml.ToModel<T>(textStream.ReadToEnd(), options: _tomlOptions);
        }
        else
        {
            value = new T();
        }

        return value;
    }

    public async Task Write(T value)
    {
        await using var file = new StreamWriter(filePath);
        await file.WriteAsync(Toml.FromModel(value, options: _tomlOptions));
        file.Close();
    }

}