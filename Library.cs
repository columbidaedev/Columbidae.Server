using System.Runtime.Serialization;
using Lagrange.Core.Common;
using Newtonsoft.Json;
using Tomlyn;

namespace Columbidae.Server;

public class Library
{
    public Proxy<PreferencesModel> Preferences { get; private set; }
    public Proxy<BotKeystore> Keystore { get; set; }

    public class PreferencesModel
    {
        public string SignServer { get; set; } = "";
        public bool ShowBotLog { get; set; } = false;
        public DeviceModel Device { get; set; } = new DeviceModel();
        public BotModel Bot { get; set; } = new BotModel();
        public AccountModel? Account { get; set; } = null;
    }

    public class BotModel
    {
        public string Protocol { get; set; } = "Linux";
    }

    public class DeviceModel
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string DeviceName { get; set; } = "Thinkpad";
        public byte[] MacAddress { get; set; } = [0xc2, 0x9f, 0x37, 0x1a, 0xd4, 0x23];
        public string SystemKernel { get; set; } = "ArchLinux rolling";
        public string KernelVersion { get; set; } = "rolling";

        public BotDeviceInfo ToDeviceInfo()
        {
            return new BotDeviceInfo
            {
                Guid = Guid,
                DeviceName = DeviceName,
                MacAddress = MacAddress,
                SystemKernel = SystemKernel,
                KernelVersion = KernelVersion
            };
        }

        public static DeviceModel FromDeviceInfo(BotDeviceInfo info)
        {
            return new DeviceModel
            {
                Guid = info.Guid,
                DeviceName = info.DeviceName,
                MacAddress = info.MacAddress,
                SystemKernel = info.SystemKernel,
                KernelVersion = info.KernelVersion
            };
        }
    }

    public class AccountModel
    {
        public uint Uin { get; set; }
        public string Password { get; set; }
    }

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

    private class TomlReadWrite<T> : IReadWrite<T> where T : class, new()
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

    private class JsonReadWrite<T> : IReadWrite<T> where T : class, new()
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

    public Library(Container container)
    {
        var prefFile = Path.Combine(container.ConfigurationRoot, "preferences.toml");
        var keystoreFile = Path.Combine(container.ConfigurationRoot, "keystore.json");
        Preferences = new Proxy<PreferencesModel>(prefFile, new TomlReadWrite<PreferencesModel>());
        Keystore = new Proxy<BotKeystore>(keystoreFile, new JsonReadWrite<BotKeystore>());
    }
}