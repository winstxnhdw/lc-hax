using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand))]
internal class InfiniteItemUsagePatch
{
    private static bool Prefix(GrabbableObject __instance)
    {
        if (Setting.EnableUnlimitedGiftBox && __instance is GiftBoxItem) return false;
        return true;
    }
}