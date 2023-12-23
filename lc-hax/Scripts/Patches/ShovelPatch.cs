#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(Shovel))]
class ShovelPatch {
    [HarmonyPatch(nameof(Shovel.HitShovel))]
    static void Prefix(ref int ___shovelHitForce) => ___shovelHitForce = Settings.ShovelHitForce;
}
