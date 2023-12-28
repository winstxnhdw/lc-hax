using GameNetcodeStuff;

namespace Hax;

public interface IEntrance { }

public static class IEntranceMixin {
    public static void EntranceTeleport(this IEntrance _, bool outside) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) return;
        localPlayer.TeleportPlayer(RoundManager.FindMainEntranceScript(outside).entrancePoint.position);
    }
}
