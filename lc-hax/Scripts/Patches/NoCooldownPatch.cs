#pragma warning disable IDE1006

using System.Collections;
using UnityEngine;
using HarmonyLib;

[HarmonyPatch]
class NoCooldownPatch {
    [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.RequireCooldown))]
    [HarmonyPrefix]
    static bool GrabbablePostFix(GrabbableObject __instance , ref bool __result) {
        if (Setting.EnableNoCooldown) {
            __result = false;
            return false;
        }

        if (__instance is Shovel or ShotgunItem or KnifeItem) {
            return true;
        }
        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(Shovel), "reelUpShovel")]
    [HarmonyPostfix]
    static IEnumerator ShovelPostfix(IEnumerator reelUpShovel) {
        while (reelUpShovel.MoveNext()) {
            if (Setting.EnableNoCooldown && reelUpShovel.Current is WaitForSeconds) continue;
            yield return reelUpShovel.Current;
        }
    }

    [HarmonyPatch(typeof(InteractTrigger), nameof(InteractTrigger.Interact))]
    [HarmonyPrefix]
    static void TriggerPostFix(InteractTrigger __instance) {

        if (__instance.GetComponent<DoorLock>() != null) return;
        
        if (__instance.transform.name.ToLower().Contains("leverswitchhandle")) return;

        __instance.interactCooldown = false;
    }
}
