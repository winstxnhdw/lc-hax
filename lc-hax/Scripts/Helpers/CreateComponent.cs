using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static T CreateComponent<T>() where T : Component => new GameObject().AddComponent<T>();
}
