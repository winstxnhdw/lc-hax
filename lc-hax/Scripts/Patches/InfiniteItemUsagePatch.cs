#region

using HarmonyLib;

#endregion

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand))]
class InfiniteItemUsagePatch {
    static bool Prefix(GrabbableObject __instance) {
        if (Setting.EnableUnlimitedGiftBox && __instance is GiftBoxItem) return false;
        return true;
    }
}
