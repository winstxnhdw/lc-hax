using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static Camera? GetCurrentCamera() =>
        Helper.LocalPlayer?.gameplayCamera != null &&
        Helper.LocalPlayer.gameplayCamera.enabled
            ? LocalPlayer.gameplayCamera
            : Helper.StartOfRound?.spectateCamera;
}
