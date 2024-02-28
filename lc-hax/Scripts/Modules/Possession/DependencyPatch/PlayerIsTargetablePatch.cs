using GameNetcodeStuff;
using HarmonyLib;
using System;
using Hax;
using UnityEngine;

[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
public static class PlayerIsTargetablePatch {
    public static bool Prefix(ref bool __result, EnemyAI __instance, PlayerControllerB playerScript, ref bool cannotBeInShip, ref bool overrideInsideFactoryCheck) {
        if (playerScript.IsDead()) {
            return true;
        }
        if (playerScript.inAnimationWithEnemy != null) {
            return true;
        }

        overrideInsideFactoryCheck = true;
        cannotBeInShip = false;

        if (PossessionMod.Instance is { IsPossessed: true } && PossessionMod.Instance.PossessedEnemy == __instance) {
            __result = true;
            return false;
        }

        if (__instance.isOutside != __instance.enemyType.isOutsideEnemy) {
            __result = true;
            return false;
        }

        return true;
    }
}
