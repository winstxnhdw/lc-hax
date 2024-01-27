#pragma warning disable IDE1006

using HarmonyLib;
using Hax;
using System.Collections.Generic;

[HarmonyPatch(typeof(GrabbableObject))]
class GrabbableDependencyPatch {

    [HarmonyPatch(nameof(GrabbableObject.Update))]
    static void Postfix(GrabbableObject __instance) {
        _ = Helper.Props.Add(__instance);
    }
}
