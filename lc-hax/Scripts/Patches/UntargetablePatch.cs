#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

class UntargetablePatch {
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
    public static bool Prefix(PlayerControllerB playerScript, ref bool __result) {
        if (!Setting.EnableUntargetable && !Setting.EnableGodMode) return true;
        if (Helper.LocalPlayer?.actualClientId != playerScript.actualClientId) return true;

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Update))]
    public static bool Prefix(ref PlayerControllerB? ___targetPlayer, ref bool ___movingTowardsTargetPlayer) {
        if (!Setting.EnableUntargetable) return true;
        if (!___targetPlayer) return true;
        if (Helper.LocalPlayer?.actualClientId != ___targetPlayer?.actualClientId) return true;

        ___targetPlayer = null;
        ___movingTowardsTargetPlayer = false;
        return false;
    }

    [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.CheckLineOfSightForLocalPlayer))]
    static bool Prefix() => !Setting.EnableUntargetable;
}
