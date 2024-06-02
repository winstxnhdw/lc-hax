using System.Runtime.CompilerServices;
using UnityEngine;

internal static partial class Extensions
{
    internal static T? Unfake<T>(this T? obj) where T : Object
    {
        return obj is null || obj.Equals(null) ? null : obj;
    }
}