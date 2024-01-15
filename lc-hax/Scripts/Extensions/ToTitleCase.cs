using System.Globalization;

public static partial class Extensions {
    static TextInfo TextInfo { get; } = new CultureInfo("en-SG", true).TextInfo;

    public static string ToTitleCase(this string str) => Extensions.TextInfo.ToTitleCase(str);
}
