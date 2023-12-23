#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(ShotgunItem))]
class UnlimitedAmmoPatch {
    [HarmonyPatch(nameof(ShotgunItem.ItemActivate))]
    static void Prefix(ShotgunItem __instance) => __instance.shellsLoaded = 2;

    [HarmonyPatch(nameof(ShotgunItem.ShootGun))]
    static void Postfix(ShotgunItem __instance) => __instance.shellsLoaded = 2;
}
