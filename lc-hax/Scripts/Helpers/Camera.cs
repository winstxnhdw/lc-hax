using UnityEngine;

namespace Hax;

internal static partial class Helper {
    internal static Camera? CurrentCamera =>
        Helper.LocalPlayer?.gameplayCamera is Camera { enabled: true } gameplayCamera
            ? gameplayCamera
            : Helper.StartOfRound?.spectateCamera;

    internal static Vector3 WorldToEyesPoint(this Camera camera, Vector3 worldPosition) {
        Vector3 screen = camera.WorldToViewportPoint(worldPosition);
        screen.x *= Screen.width;
        screen.y *= Screen.height;
        screen.y = Screen.height - screen.y;

        return screen;
    }
}
