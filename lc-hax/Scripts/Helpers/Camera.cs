using UnityEngine;

namespace Hax;

internal static partial class Helper {
    internal static Camera? CurrentCamera =>
        Helper.LocalPlayer?.gameplayCamera is Camera { enabled: true } gameplayCamera
            ? gameplayCamera
            : Helper.StartOfRound?.spectateCamera;
}
