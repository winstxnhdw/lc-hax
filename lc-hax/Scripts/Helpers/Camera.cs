using UnityEngine;

static partial class Helper {
    internal static Camera? CurrentCamera => Helper.StartOfRound?.activeCamera;

    internal static Vector3 WorldToEyesPoint(this Camera camera, Vector3 worldPosition, Rect screenBounds) {
        Vector3 screen = camera.WorldToViewportPoint(worldPosition);
        screen.x *= screenBounds.width;
        screen.y = screenBounds.height - (screen.y * screenBounds.height);

        return screen;
    }
}
