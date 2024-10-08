using Columbidae.Server.Core.Preferences;
using Tomlyn;

namespace Columbidae.Server.Preferences;

public class TomlReadWrite<T>(string filePath) : IFileReadWrite<T>
    where T : class, new()
{
    private readonly TomlModelOptions _tomlOptions = new()
    {
        ConvertToModel = (o, type) =>
        {
            if (type == typeof(Guid) && o is string s) return Guid.Parse(s);

            return null;
        },
        ConvertToToml = o => o is Guid ? o.ToString() : null
    };

    public string FilePath => filePath;


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
        await file.WriteAsync(Toml.FromModel(value, _tomlOptions));
        file.Close();
    }
}