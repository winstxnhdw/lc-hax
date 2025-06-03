#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(EnemyAI))]
class UntargetableEnemyPatch {
    [HarmonyPatch(nameof(EnemyAI.PlayerIsTargetable))]
    static void Postfix(PlayerControllerB playerScript, ref bool __result) {
        if (!Setting.EnableUntargetable) return;
        if (!playerScript.IsSelf()) return;

        __result = false;
    }

    [HarmonyPatch(nameof(EnemyAI.Update))]
    static bool Prefix(EnemyAI __instance) {
        if (!Setting.EnableUntargetable) return true;
        if (!__instance.targetPlayer.IsSelf()) return true;

        __instance.targetPlayer = null;
        __instance.movingTowardsTargetPlayer = false;
        return false;
    }
}

[HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.CheckLineOfSightForLocalPlayer))]
class UntargetableNutcrackerPatch {
    static bool Prefix() => !Setting.EnableUntargetable;
}
