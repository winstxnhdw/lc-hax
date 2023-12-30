using System.Collections.Generic;
using System.Linq;

namespace Hax;

public static partial class Extensions {
    public static IEnumerable<int> Range(this int end, int start) {
        return Enumerable.Range(start, end);
    }

    public static IEnumerable<int> Range(this int end) {
        return end.Range(0);
    }
}
