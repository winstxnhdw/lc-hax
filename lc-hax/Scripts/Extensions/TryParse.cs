internal static partial class Extensions
{
    internal static bool TryParse(this string? value, ushort defaultValue, out ushort result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = defaultValue;
            return true;
        }

        return ushort.TryParse(value, out result);
    }

    internal static bool TryParse(this string? value, ulong defaultValue, out ulong result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = defaultValue;
            return true;
        }

        return ulong.TryParse(value, out result);
    }
}