namespace Hax;

internal static partial class Helper {
    internal static Camera? CurrentCamera =>
        Helper.LocalPlayer?.gameplayCamera is Camera gameplayCamera && gameplayCamera.enabled
            ? gameplayCamera
            : Helper.StartOfRound?.spectateCamera;
}
