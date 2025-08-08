using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand))]
sealed class InfiniteItemUsagePatch {
    static bool Prefix() => false;
}
