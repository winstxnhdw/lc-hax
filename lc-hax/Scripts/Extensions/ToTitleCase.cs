using System.Globalization;

static partial class Extensions {
    static TextInfo TextInfo { get; } = new CultureInfo("en-SG", true).TextInfo;

    internal static string ToTitleCase(this string str) => Extensions.TextInfo.ToTitleCase(str);
}
