using GameNetcodeStuff;

namespace Hax;

public static partial class Helper {
    static ManualCameraRenderer? ManualCameraRenderer => Helper.StartOfRound?.mapScreen;

    public static void SwitchRadarTarget(int playerClientId) {
        Helper.ManualCameraRenderer?.SwitchRadarTargetServerRpc(playerClientId);
    }

    public static void SwitchRadarTarget(PlayerControllerB player) {
        Helper.SwitchRadarTarget((int)player.playerClientId);
    }

    public static bool IsRadarTarget(ulong playerClientId) {
        return Helper.ManualCameraRenderer?.targetTransformIndex == (int)playerClientId;
    }
}
