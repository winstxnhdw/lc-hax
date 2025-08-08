using System;
using System.Runtime.CompilerServices;

sealed class Arguments {
    internal required string[] Span { private get; init; }

    internal int Length => this.Span.Length;

    internal string? this[uint index] => index >= this.Span.Length ? null : this.Span[index];

    internal Arguments this[Range range] => new() {
        Span = this.Span[range]
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<string>(Arguments stringArray) => stringArray.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Arguments(string[] array) => new() {
        Span = array
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator string[](Arguments stringArray) => stringArray.Span;
}
