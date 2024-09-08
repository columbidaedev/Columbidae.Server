using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Columbidae.Server.Core.Bot;

internal static class HashUtility
{
    public static string Md5(this byte[] bytes)
    {
        var buf = MD5.HashData(bytes);
        return buf.Hex();
    }

    public static string Hex(this IEnumerable<byte> bytes)
    {
        var sb = new StringBuilder();
        foreach (var b in bytes) sb.Append(b.ToString("x2"));

        return sb.ToString();
    }

    public static byte[] UnHex(this string hex)
    {
        if (hex.Length % 2 != 0) throw new ArgumentException("Invalid hex string");

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < hex.Length; i += 2) bytes[i / 2] = byte.Parse(hex.Substring(i, 2), NumberStyles.HexNumber);
        return bytes;
    }
}