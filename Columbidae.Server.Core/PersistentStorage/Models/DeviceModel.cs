using Lagrange.Core.Common;

namespace Columbidae.Server.Core.PersistentStorage.Models;

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