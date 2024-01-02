using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand))]
class InfiniteItemUsagePatch {
    static bool Prefix() => false;
}
