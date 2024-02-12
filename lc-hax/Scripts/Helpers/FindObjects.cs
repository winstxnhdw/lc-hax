namespace Hax;

internal static partial class Helper {
    internal static T? FindObject<T>() where T : Component => Object.FindAnyObjectByType<T>();

    internal static T[] FindObjects<T>(FindObjectsSortMode sortMode = FindObjectsSortMode.None) where T : Component =>
        Object.FindObjectsByType<T>(sortMode);
}
