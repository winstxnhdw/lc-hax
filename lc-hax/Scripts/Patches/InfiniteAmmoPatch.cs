#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ItemActivate))]
class InfiniteShotgunAmmoPatch {
    static void Prefix(ShotgunItem __instance, EnemyAI? ___heldByEnemy) {
        if (___heldByEnemy is not null) return;
        __instance.shellsLoaded = 3;
    }
}
