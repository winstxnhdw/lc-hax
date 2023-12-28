#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
class UntargetablePatch {
    public static bool Prefix(PlayerControllerB playerScript, ref bool __result) {
        if (!Setting.EnableUntargetable && !Setting.EnableGodMode) return true;
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) return true;
        if (localPlayer.actualClientId != playerScript.actualClientId) return true;

        __result = false;
        return false;
    }
}
