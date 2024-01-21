#pragma warning disable IDE1006

using HarmonyLib;
using System.Collections.Generic;

[HarmonyPatch(typeof(GrabbableObject))]
class GrabbableDependencyPatch {
    public static HashSet<GrabbableObject> ActiveProps { get; } = [];

    [HarmonyPatch(nameof(GrabbableObject.Update))]
    static void Postfix(GrabbableObject __instance) {
        _ = ActiveProps.Add(__instance);
    }
}
