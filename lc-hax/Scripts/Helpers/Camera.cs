using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public static partial class Helper {
    public static Camera? CurrentCamera =>
        Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer) &&
        localPlayer.gameplayCamera.IsNotNull(out Camera gameplayCamera) &&
        gameplayCamera.enabled
            ? gameplayCamera
            : Helper.StartOfRound?.spectateCamera;
}
