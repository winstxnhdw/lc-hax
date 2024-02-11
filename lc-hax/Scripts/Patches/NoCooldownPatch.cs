#pragma warning disable IDE1006

using System.Collections;
using HarmonyLib;
using Hax;
using UnityEngine;

[HarmonyPatch]
class NoCooldownPatch {
    [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.RequireCooldown))]
    static void Postfix(ref bool __result) {
        if (!Setting.EnableNoCooldown) return;
        __result = false;
    }

    [HarmonyPatch(typeof(Shovel), "reelUpShovel")]
    static IEnumerator Postfix(IEnumerator reelUpShovel) {
        while (reelUpShovel.MoveNext()) {
            if (Setting.EnableNoCooldown && reelUpShovel.Current is WaitForSeconds) continue;
            yield return reelUpShovel.Current;
        }
    }
}
