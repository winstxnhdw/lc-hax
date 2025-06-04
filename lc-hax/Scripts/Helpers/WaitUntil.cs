using System;
using System.Threading;
using System.Threading.Tasks;

static partial class Helper {
    internal static async Task WaitUntil(Func<bool> predicate, CancellationToken cancellationToken) {
        while (!predicate()) {
            if (cancellationToken.IsCancellationRequested) break;
            await Task.Yield();
        }
    }
}
