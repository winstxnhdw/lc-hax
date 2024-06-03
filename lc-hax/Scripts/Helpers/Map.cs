#region

using GameNetcodeStuff;

#endregion

namespace Hax;

static partial class Helper {
    static ManualCameraRenderer? ManualCameraRenderer => StartOfRound?.mapScreen;

    internal static bool IsRadarTarget(ulong playerClientId) =>
        ManualCameraRenderer?.targetTransformIndex == unchecked((int)playerClientId);

    internal static void SwitchRadarTarget(int playerClientId) =>
        ManualCameraRenderer?.SwitchRadarTargetServerRpc(playerClientId);

    internal static void SwitchRadarTarget(PlayerControllerB player) => SwitchRadarTarget(player.GetPlayerId());
}
