#pragma warning disable IDE1006
using HarmonyLib;
using GameNetcodeStuff;
using Hax;
using System;
using UnityEngine;

[HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.HitEnemy))]
class EnemyHealthPatch{
    public static void Postfix(EnemyAI __instance, int force, PlayerControllerB playerWhoHit, bool playHitSFX, int hitID) {
        if (!playerWhoHit.IsSelf()) return;
        if(__instance.isEnemyDead) return;
        int originalhealth = GetOriginalHealth(__instance);
        Helper.DisplayFlatHudMessage($"Remaining Hits to kill {__instance.enemyType.enemyName} : {__instance.enemyHP}/{originalhealth}");
    }

    
    private static int GetOriginalHealth(EnemyAI instance)
    {
        GameObject prefab = instance.enemyType.enemyPrefab;
        if (prefab == null) return instance.enemyHP;
        EnemyAI enemyAI = prefab.GetComponent<EnemyAI>();
        if (enemyAI == null) return instance.enemyHP;
        return enemyAI.enemyHP;
    }
}
