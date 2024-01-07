using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static T? FindObject<T>() where T : Component => Object.FindAnyObjectByType<T>();

    public static T[] FindObjects<T>(FindObjectsSortMode sortMode = FindObjectsSortMode.None) where T : Component =>
        Object.FindObjectsByType<T>(sortMode);
}
