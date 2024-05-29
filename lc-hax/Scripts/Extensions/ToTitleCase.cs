using System.Globalization;

internal static partial class Extensions
{
    private static TextInfo TextInfo { get; } = new CultureInfo("en-SG", true).TextInfo;

    internal static string ToTitleCase(this string str)
    {
        return TextInfo.ToTitleCase(str);
    }
}