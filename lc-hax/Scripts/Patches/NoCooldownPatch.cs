#pragma warning disable IDE1006

using System.Collections;
using UnityEngine;
using HarmonyLib;
using Hax;

[HarmonyPatch]
class NoCooldownPatch {
    [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.RequireCooldown))]
    static bool Prefix(ref bool __result) {
        if (!Setting.EnableNoCooldown) return true;

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(Shovel), "reelUpShovel")]
    static IEnumerator Postfix(IEnumerator reelUpShovel) {
        while (reelUpShovel.MoveNext()) {
            if (Setting.EnableNoCooldown && reelUpShovel.Current is WaitForSeconds) continue;
            yield return reelUpShovel.Current;
        }
    }
}
