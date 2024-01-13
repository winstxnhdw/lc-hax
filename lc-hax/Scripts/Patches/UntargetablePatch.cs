#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch()]
class UntargetablePatch {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
    public static bool PlayerPrefix(PlayerControllerB playerScript, ref bool __result) {
        if (!Setting.EnableUntargetable && !Setting.EnableGodMode) return true;
        if (Helper.LocalPlayer?.actualClientId != playerScript.actualClientId) return true;

        __result = false;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.Update))]
    public static bool EnemyAIPrefix(EnemyAI __instance) {
        if (!Setting.EnableUntargetable) return true;
        if (!__instance.targetPlayer) return true;
        if (Helper.LocalPlayer?.actualClientId != __instance.targetPlayer.actualClientId) return true;

        __instance.targetPlayer = null;
        __instance.movingTowardsTargetPlayer = false;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.CheckLineOfSightForLocalPlayer))]
    static bool NutcrackerPrefix() => !Setting.EnableUntargetable;
}

//TODO: PufferAI.Update
//__instance.playerIsInLOS = false;
//__instance.closestSeenPlayer = null;
