using System.Runtime.Serialization;
using Columbidae.Server.Core.Preferences;
using Newtonsoft.Json;

namespace Columbidae.Server.Preferences;

public class JsonReadWrite<T>(string filePath) : IFileReadWrite<T> where T : class, new()
{
    public string FilePath => filePath;

    public T Read()
    {
        T? value;
        if (File.Exists(filePath))
        {
            var text = File.OpenText(filePath);
            value = JsonConvert.DeserializeObject<T>(text.ReadToEnd());
            if (value == null) throw new SerializationException($"Illegal JSON format: {filePath}");
        }
        else
        {
            value = new T();
        }

        return value;
    }

    public async Task Write(T value)
    {
        var json = JsonConvert.SerializeObject(value);
        await using var text = new StreamWriter(filePath);
        await text.WriteAsync(json);
        text.Close();
    }
}