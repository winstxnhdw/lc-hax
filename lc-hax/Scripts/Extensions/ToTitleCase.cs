#region

using System.Globalization;

#endregion

static partial class Extensions {
    static TextInfo TextInfo { get; } = new CultureInfo("en-SG", true).TextInfo;

    internal static string ToTitleCase(this string str) => TextInfo.ToTitleCase(str);
}
