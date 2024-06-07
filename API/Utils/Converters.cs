using static System.Convert;

namespace API.Utils;


public static class Converters
{
    public static byte[]? ConvertStringBase64ToBytes(string? value)
    {
        try
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            return FromBase64String(value);
        }
        catch
        {
            return default;
        }

    }

    public static bool TryConvertStringBase64ToBytes(string? value, out byte[]? bytes)
    {
        bytes = default;

        try
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            bytes = FromBase64String(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}