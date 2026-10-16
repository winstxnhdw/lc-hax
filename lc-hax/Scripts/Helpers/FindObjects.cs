using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

static partial class Helper {
    internal static T? FindObject<T>() where T : Component => Object.FindAnyObjectByType<T>();

    internal static T[] FindObjects<T>(FindObjectsSortMode sortMode = FindObjectsSortMode.None) where T : Component =>
        Object.FindObjectsByType<T>(sortMode);

    internal static async Task<T?> FindObject<T>(CancellationToken cancellationToken) where T : Component {
        while (true) {
            cancellationToken.ThrowIfCancellationRequested();

            if (Helper.FindObject<T>() is T obj) {
                return obj;
            }

            await Task.Yield();
        }
    }
}
