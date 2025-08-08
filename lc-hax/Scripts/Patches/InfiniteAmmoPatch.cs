#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ItemActivate))]
sealed class InfiniteShotgunAmmoPatch {
    static void Prefix(ShotgunItem __instance, ref EnemyAI? ___heldByEnemy) {
        if (__instance.isReloading) return;
        if (___heldByEnemy is not null) return;
        __instance.shellsLoaded = 3;
    }
}
