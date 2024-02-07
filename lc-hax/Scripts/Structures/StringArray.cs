using System;
using System.Runtime.CompilerServices;

internal readonly ref struct StringArray {
    ReadOnlySpan<string> Span { get; }

    internal StringArray(ReadOnlySpan<string> span) => this.Span = span;

    internal int Length => this.Span.Length;

    internal string? this[ushort index] => index >= this.Span.Length ? null : this.Span[index];

    internal StringArray this[Range range] => new(this.Span[range]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<string>(StringArray stringArray) => stringArray.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator StringArray(ReadOnlySpan<string> span) => new(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator StringArray(string[] array) => new(array);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator string[](StringArray stringArray) => stringArray.Span.ToArray();
}
