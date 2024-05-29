using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

internal static partial class Extensions
{
    public static bool IsEmpty<T>(this List<T> list)
    {
        return list == null || list.Count == 0;
    }

    public static bool IsEmpty(this Array list)
    {
        return list == null || list.Length == 0;
    }

    public static bool IsNotEmpty<T>(this List<T> list)
    {
        return list.Count != 0;
    }

    public static bool IsNotEmpty(this Array list)
    {
        return list.Length != 0;
    }

    public static bool IsNull<T>(this T obj) where T : class
    {
        return obj == null;
    }

    public static bool IsNull<T>(this T? obj) where T : struct
    {
        return !obj.HasValue;
    }

    public static bool IsNotNull<T>(this T obj) where T : class
    {
        return obj != null;
    }

    public static bool IsNotNull<T>(this T? obj) where T : struct
    {
        return obj.HasValue;
    }

    public static bool IsNotNullOrEmptyOrWhiteSpace(this string obj)
    {
        return !string.IsNullOrEmpty(obj) && !string.IsNullOrWhiteSpace(obj);
    }

    public static bool IsNullOrEmptyOrWhiteSpace(this string obj)
    {
        return string.IsNullOrEmpty(obj) && string.IsNullOrWhiteSpace(obj);
    }

    internal static string ReplaceWholeWord(this string original, string wordToFind, string replacement,
        RegexOptions regexOptions = RegexOptions.IgnoreCase)
    {
        return Regex.Replace(original, wordToFind, replacement, regexOptions);
    }

    internal static bool IsMatch(this string value, string wordToFind,
        RegexOptions regexOptions = RegexOptions.IgnoreCase)
    {
        return Regex.IsMatch(value, wordToFind, regexOptions);
    }

    internal static bool IsMatchWholeWord(this string value, string wordToFind,
        RegexOptions regexOptions = RegexOptions.IgnoreCase)
    {
        return Regex.IsMatch(value, $@"\b{wordToFind}\b", regexOptions);
    }

    internal static string RemoveWhitespace(this string str)
    {
        return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
    }

    internal static IEnumerable<string> ReadLines(this string s)
    {
        string line;
        using StringReader sr = new(s);
        while ((line = sr.ReadLine()) != null)
            yield return line;
    }


    internal static bool IsPositive(this int number)
    {
        return number > 0;
    }

    internal static bool IsDigitsOnly(this string s)
    {
        var len = s.Length;
        for (var i = 0; i < len; ++i)
        {
            var c = s[i];
            if (c is < '0' or > '9')
                return false;
        }

        return true;
    }

    internal static bool IsNegative(this int number)
    {
        return number < 0;
    }

    internal static bool CheckRange(this float num, float min, float max)
    {
        return num > min && num < max;
    }

    internal static string Truncate(this string value, int max_length)
    {
        if (!string.IsNullOrEmpty(value) && value.Length > max_length)
            return value[..max_length] + "…";
        return value;
    }

    internal static void AddWithoutRepetition<T>(this List<T> target, List<T> source)
    {
        var sourceCount = source.Count;
        for (var k = 0; k < sourceCount; k++)
        {
            var entry = source[k];
            if (entry != null && !target.Contains(entry)) target.Add(entry);
        }
    }

    internal static string ConvertToString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
    }
}