using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static Camera? CurrentCamera =>
        Helper.LocalPlayer?.gameplayCamera.IsNotNull(out Camera gameplayCamera) is true &&
        gameplayCamera.enabled
            ? gameplayCamera
            : Helper.StartOfRound?.spectateCamera;
}
