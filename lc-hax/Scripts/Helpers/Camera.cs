using UnityEngine;

namespace Hax;

static partial class Helper {
    internal static Camera? CurrentCamera =>
        (HaxCamera.Instance?.CustomCamera?.enabled == true)
            ? HaxCamera.Instance.CustomCamera
            : (!Helper.LocalPlayer?.IsDead() ?? false) && (Helper.LocalPlayer?.gameplayCamera?.enabled == true)
                ? Helper.LocalPlayer.gameplayCamera
                : (Helper.StartOfRound?.spectateCamera?.enabled == true)
                    ? Helper.StartOfRound.spectateCamera
                    : null;


    internal static Vector3 WorldToEyesPoint(this Camera camera, Vector3 worldPosition) {
        Vector3 screen = camera.WorldToViewportPoint(worldPosition);
        screen.x *= Screen.width;
        screen.y *= Screen.height;
        screen.y = Screen.height - screen.y;

        return screen;
    }
}
