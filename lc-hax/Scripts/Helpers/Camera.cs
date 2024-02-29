using UnityEngine;

namespace Hax;

static partial class Helper {
    internal static Camera? CurrentCamera {
        get {
            return HaxCamera.Instance?.HaxCamContainer?.activeSelf == true
                ? HaxCamera.Instance.CustomCamera
                : Helper.StartOfRound?.activeCamera;
        }
    }

    internal static Vector3 WorldToEyesPoint(this Camera camera, Vector3 worldPosition) {
        Vector3 screen = camera.WorldToViewportPoint(worldPosition);
        screen.x *= Screen.width;
        screen.y *= Screen.height;
        screen.y = Screen.height - screen.y;

        return screen;
    }
}
