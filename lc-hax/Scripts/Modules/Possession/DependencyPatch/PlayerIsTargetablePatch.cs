using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
public static class PlayerIsTargetablePatch {
    public static bool Prefix(ref bool __result, EnemyAI __instance, PlayerControllerB playerScript) {
        if (!playerScript.isPlayerControlled) return true;
        if (playerScript.isPlayerDead) return true;
        if (playerScript.inAnimationWithEnemy != null) return true;
        if (__instance.isOutside == __instance.enemyType.isOutsideEnemy) return true;
        __result = true;
        return false;

    }
}
