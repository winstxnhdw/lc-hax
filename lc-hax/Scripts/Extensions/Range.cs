using System.Collections.Generic;
using System.Linq;

static partial class Extensions {
    internal static IEnumerable<int> Range(this int end) => Enumerable.Range(0, end);
}
