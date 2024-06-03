#region

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

#endregion

[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
public static class PlayerIsTargetablePatch {
    public static bool Prefix(ref bool __result, EnemyAI __instance, PlayerControllerB playerScript,
        ref bool cannotBeInShip, ref bool overrideInsideFactoryCheck) {
        if (PossessionMod.Instance is { IsPossessed: true } && PossessionMod.Instance.PossessedEnemy == __instance) {
            if (playerScript.IsDead()) return true;
            __result = true;
            return false;
        }


        if (__instance.isOutside != __instance.enemyType.isOutsideEnemy) {
            overrideInsideFactoryCheck = true;
            cannotBeInShip = false;
        }

        return true;
    }
}
