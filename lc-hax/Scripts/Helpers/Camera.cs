using UnityEngine;

namespace Hax;

public static partial class Helpers {
    public static Camera? GetCurrentCamera() =>
        Helpers.LocalPlayer?.gameplayCamera != null &&
        Helpers.LocalPlayer.gameplayCamera.enabled
            ? LocalPlayer.gameplayCamera
            : Helpers.StartOfRound?.spectateCamera;
}
