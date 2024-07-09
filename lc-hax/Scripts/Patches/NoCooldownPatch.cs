#pragma warning disable IDE1006

#region

using System.Collections;
using HarmonyLib;
using UnityEngine;

#endregion

[HarmonyPatch]
class NoCooldownPatch {
    [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.RequireCooldown))]
    [HarmonyPrefix]
    static bool GrabbablePostFix(GrabbableObject __instance, ref bool __result) {
        if (Setting.EnableNoCooldown) {
            __result = false;
            return false;
        }

        if (__instance is Shovel or ShotgunItem or KnifeItem) return true;
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
        // check also if there's one child and if that  contains a lever switch handle transform
        if (__instance.transform.childCount == 1 && __instance.transform.GetChild(0).name.ToLower().Contains("leverswitchhandle")) return;
        if (__instance.transform.root.name.ToLower().Contains("companycruiser")) return;

        __instance.interactCooldown = false;
    }
}
