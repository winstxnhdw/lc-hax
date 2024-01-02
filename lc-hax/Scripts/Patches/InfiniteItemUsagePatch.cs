using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand))]
class InfiniteItemUsagePatch {
    static bool Prefix() => false;
}
