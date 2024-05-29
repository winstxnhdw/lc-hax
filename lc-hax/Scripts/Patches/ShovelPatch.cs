#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(Shovel), nameof(Shovel.HitShovel))]
internal class ShovelPatch
{
    private static void Prefix(Shovel __instance)
    {
        __instance.shovelHitForce = State.ShovelHitForce;
    }
}