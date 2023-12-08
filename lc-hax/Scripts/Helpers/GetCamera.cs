using UnityEngine;

namespace Hax;

public static partial class Helpers {
    public static Camera? GetCurrentCamera() {
        if (LocalPlayer == null) return null;

        if (LocalPlayer.gameplayCamera != null
            && LocalPlayer.gameplayCamera.enabled) return LocalPlayer.gameplayCamera;

        if (StartOfRound.Instance != null)
            if (StartOfRound.Instance.spectateCamera != null
                && StartOfRound.Instance.spectateCamera.enabled) return StartOfRound.Instance.spectateCamera;

        return null;
    }
}
