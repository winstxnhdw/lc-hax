using UnityEngine;

namespace Hax;

static partial class Helper {
    internal static Vector2 GetScreenCentre() => new Vector2(Screen.width, Screen.height) * 0.5f;
}
