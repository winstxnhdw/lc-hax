using GameNetcodeStuff;

namespace Hax;

public static partial class Helpers {
    public static void SwitchRadarTarget(ulong playerClientId) {
        StartOfRound.Instance.mapScreen.SwitchRadarTargetServerRpc((int)playerClientId);
    }

    public static void SwitchRadarTarget(PlayerControllerB player) {
        Helpers.SwitchRadarTarget(player.playerClientId);
    }
}
