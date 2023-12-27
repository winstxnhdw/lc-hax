#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(Shovel), nameof(Shovel.HitShovel))]
class ShovelPatch {
    static void Prefix(ref int ___shovelHitForce) => ___shovelHitForce = Settings.ShovelHitForce;
}
