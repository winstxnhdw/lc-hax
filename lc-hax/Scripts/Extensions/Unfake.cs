using System.Runtime.CompilerServices;
using UnityEngine;

static partial class Extensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T? Unfake<T>(this T? obj) where T : Object => obj is null || obj.Equals(null) ? null : obj;
}
