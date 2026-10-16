using System;
using System.Threading;
using System.Threading.Tasks;

static partial class Helper {
    internal static Task DelaySafely(TimeSpan delay, CancellationToken cancellationToken = default) {
        Task delayTask = Task.Delay(delay, cancellationToken);
        return delayTask.IsCompletedSuccessfully ? Helper.YieldWithCancellation(cancellationToken) : delayTask;
    }

    static async Task YieldWithCancellation(CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
    }
}
