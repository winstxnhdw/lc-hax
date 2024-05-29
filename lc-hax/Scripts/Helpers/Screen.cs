using UnityEngine;

namespace Hax;

internal static partial class Helper
{
    internal static Vector2 GetScreenCentre()
    {
        return new Vector2(Screen.width, Screen.height) * 0.5f;
    }
}