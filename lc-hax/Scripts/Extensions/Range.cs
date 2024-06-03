#region

using System.Collections.Generic;
using System.Linq;

#endregion

static partial class Extensions {
    internal static IEnumerable<int> Range(this int end) => Enumerable.Range(0, end);
}
