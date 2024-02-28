using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
public static class PlayerIsTargetablePatch {
    public static bool Prefix(ref bool __result, EnemyAI __instance, PlayerControllerB playerScript,
        ref bool cannotBeInShip, ref bool overrideInsideFactoryCheck) {
        overrideInsideFactoryCheck = true;
        cannotBeInShip = false;

        if (SkipTargetCheck(playerScript, __instance)) {
            __result = true;
            return false;
        }

        return true;
    }

    private static bool SkipTargetCheck(PlayerControllerB playerScript, EnemyAI enemyAI) {
        return !playerScript.isPlayerControlled ||
               playerScript.isPlayerDead ||
               playerScript.inAnimationWithEnemy != null ||
               enemyAI.isOutside == enemyAI.enemyType.isOutsideEnemy;
    }
}
