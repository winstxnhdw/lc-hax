#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(Shovel), nameof(Shovel.HitShovel))]
class ShovelPatch {
    static void Prefix(ref int ___shovelHitForce) => ___shovelHitForce = Setting.ShovelHitForce;
}
