using System.Collections.Generic;
using System.Linq;

internal static partial class Extensions
{
    internal static IEnumerable<int> Range(this int end)
    {
        return Enumerable.Range(0, end);
    }
}