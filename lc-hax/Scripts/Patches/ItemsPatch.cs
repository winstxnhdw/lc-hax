using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Start))]
class ItemsPatch {
    static void Postfix(ref GrabbableObject __instance) {
        if(__instance != null && __instance.itemProperties != null) {
            __instance.itemProperties.canBeGrabbedBeforeGameStart = true;
            __instance.itemProperties.twoHanded = false;
        }
    }
}
