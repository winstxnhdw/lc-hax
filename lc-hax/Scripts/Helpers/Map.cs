using GameNetcodeStuff;

namespace Hax;

internal static partial class Helper {
    static ManualCameraRenderer? ManualCameraRenderer => Helper.StartOfRound?.mapScreen;

    internal static bool IsRadarTarget(ulong playerClientId) => Helper.ManualCameraRenderer?.targetTransformIndex == unchecked((int)playerClientId);

    internal static void SwitchRadarTarget(int playerClientId) => Helper.ManualCameraRenderer?.SwitchRadarTargetServerRpc(playerClientId);

    internal static void SwitchRadarTarget(PlayerControllerB player) => Helper.SwitchRadarTarget(player.PlayerIndex());
}
