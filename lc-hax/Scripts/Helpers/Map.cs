using GameNetcodeStuff;

namespace Hax;

public static partial class Helper {
    static ManualCameraRenderer? ManualCameraRenderer => Helper.StartOfRound?.mapScreen;

    public static void SwitchRadarTarget(ulong playerClientId) {
        Helper.ManualCameraRenderer?.SwitchRadarTargetServerRpc((int)playerClientId);
    }

    public static void SwitchRadarTarget(PlayerControllerB player) {
        Helper.SwitchRadarTarget(player.playerClientId);
    }

    public static bool IsRadarTarget(ulong playerClientId) {
        return Helper.ManualCameraRenderer?.targetTransformIndex == (int)playerClientId;
    }
}
