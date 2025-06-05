using ZLinq;

static partial class Extensions {
    internal static ValueEnumerable<ZLinq.Linq.FromRange, int> Range(this int end) => ValueEnumerable.Range(0, end);
}
