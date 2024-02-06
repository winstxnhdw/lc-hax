using UnityEngine;

namespace Hax;

internal static partial class Helper {
    internal static Vector2 GetScreenCentre() => new Vector2(Screen.width, Screen.height) * 0.5f;

    internal static Vector3 WorldToEyesPoint(this Camera camera, Vector3 worldPosition) {
        Vector3 screen = camera.WorldToViewportPoint(worldPosition);
        screen.x *= Screen.width;
        screen.y *= Screen.height;
        screen.y = Screen.height - screen.y;

        return screen;
    }
}
