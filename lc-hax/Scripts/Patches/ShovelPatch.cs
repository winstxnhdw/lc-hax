#pragma warning disable IDE1006

#region

using HarmonyLib;

#endregion

[HarmonyPatch(typeof(Shovel), nameof(Shovel.HitShovel))]
class ShovelPatch {
    static void Prefix(Shovel __instance) => __instance.shovelHitForce = State.ShovelHitForce;
}
