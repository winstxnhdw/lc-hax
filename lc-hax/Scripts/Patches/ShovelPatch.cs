using HarmonyLib;

[HarmonyPatch(typeof(Shovel), nameof(Shovel.HitShovel))]
sealed class ShovelPatch {
    static void Prefix(Shovel __instance) => __instance.shovelHitForce = State.ShovelHitForce;
}
