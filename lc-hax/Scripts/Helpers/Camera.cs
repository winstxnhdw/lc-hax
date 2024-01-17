using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static Camera? CurrentCamera =>
        Helper.LocalPlayer?.gameplayCamera is Camera gameplayCamera && gameplayCamera.enabled
            ? gameplayCamera
            : Helper.StartOfRound?.spectateCamera;
}
