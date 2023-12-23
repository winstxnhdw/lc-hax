#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(ShotgunItem))]
class UnlimitedAmmoPatch {
    [HarmonyPatch(nameof(ShotgunItem.ItemActivate))]
    static void Prefix(ref int ___shellsLoaded) => ___shellsLoaded = 2;

    [HarmonyPatch(nameof(ShotgunItem.ShootGun))]
    static void Postfix(ref int ___shellsLoaded) => ___shellsLoaded = 2;
}
