using System.Collections.Generic;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.CanPlayerScan))]
sealed class PhantomScanPatch {
    static bool Prefix(ref bool __result) {
        if (!Setting.EnablePhantom) return true;

        __result = false;
        return false;
    }
}

[HarmonyPatch]
sealed class PhantomInputPatch {
    static IEnumerable<MethodBase> TargetMethods() => [
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.ActivateItem_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.ItemSecondaryUse_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.ItemTertiaryUse_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.QEItemInteract_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.Interact_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.Jump_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.Crouch_performed))
    ];

    static bool Prefix(PlayerControllerB __instance) => !Setting.EnablePhantom || !__instance.IsSelf();
}
