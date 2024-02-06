using System.Collections.Generic;
using System.Linq;

public static partial class Extensions {
    public static IEnumerable<int> Range(this int end) => Enumerable.Range(0, end);
}
