using GameNetcodeStuff;

namespace Hax;

internal static partial class Helper
{
    private static ManualCameraRenderer? ManualCameraRenderer => StartOfRound?.mapScreen;

    internal static bool IsRadarTarget(ulong playerClientId)
    {
        return ManualCameraRenderer?.targetTransformIndex == unchecked((int)playerClientId);
    }

    internal static void SwitchRadarTarget(int playerClientId)
    {
        ManualCameraRenderer?.SwitchRadarTargetServerRpc(playerClientId);
    }

    internal static void SwitchRadarTarget(PlayerControllerB player)
    {
        SwitchRadarTarget(player.GetPlayerID());
    }
}