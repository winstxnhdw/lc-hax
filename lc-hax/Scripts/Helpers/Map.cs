using GameNetcodeStuff;

namespace Hax;

public static partial class Helper {
    static ManualCameraRenderer? ManualCameraRenderer => Helper.StartOfRound?.mapScreen;

    public static bool IsRadarTarget(ulong playerClientId) => Helper.ManualCameraRenderer?.targetTransformIndex == unchecked((int)playerClientId);

    public static void SwitchRadarTarget(int playerClientId) => Helper.ManualCameraRenderer?.SwitchRadarTargetServerRpc(playerClientId);

    public static void SwitchRadarTarget(PlayerControllerB player) => Helper.SwitchRadarTarget(player.ClientId());
}
