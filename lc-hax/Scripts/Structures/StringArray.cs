using System;
using System.Runtime.CompilerServices;

internal readonly ref struct StringArray
{
    internal required ReadOnlySpan<string> Span { get; init; }

    internal int Length => Span.Length;

    internal string? this[ushort index] => index >= Span.Length ? null : Span[index];

    internal StringArray this[Range range] => new()
    {
        Span = Span[range]
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<string>(StringArray stringArray)
    {
        return stringArray.Span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator StringArray(ReadOnlySpan<string> span)
    {
        return new StringArray
        {
            Span = span
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator StringArray(string[] array)
    {
        return new StringArray
        {
            Span = array
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator string[](StringArray stringArray)
    {
        return stringArray.Span.ToArray();
    }
}