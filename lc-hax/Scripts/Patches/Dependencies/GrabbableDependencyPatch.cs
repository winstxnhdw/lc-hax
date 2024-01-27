#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(GrabbableObject))]
class GrabbableDependencyPatch {
    [HarmonyPatch(nameof(GrabbableObject.Update))]
    static void Postfix(GrabbableObject __instance) {
        _ = Helper.Grabbables.Add(__instance);
    }
}
