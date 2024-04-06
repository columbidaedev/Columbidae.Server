using System.Runtime.Serialization;
using Columbidae.Server.Core.PersistentStorage;
using Newtonsoft.Json;

namespace Columbidae.Server.PersistentStorage;

public class JsonReadWrite<T> : IReadWrite<T> where T : class, new()
{
    public T Read(string filepath)
    {
        T? value;
        if (File.Exists(filepath))
        {
            var text = File.OpenText(filepath);
            value = JsonConvert.DeserializeObject<T>(text.ReadToEnd());
            if (value == null)
            {
                throw new SerializationException($"Illegal JSON format: {filepath}");
            }
        }
        else
        {
            value = new T();
        }

        return value;
    }

    public async Task Write(string filepath, T value)
    {
        var json = JsonConvert.SerializeObject(value);
        await using var text = new StreamWriter(filepath);
        await text.WriteAsync(json);
        text.Close();
    }
}