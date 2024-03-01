#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ItemActivate))] // Load shell if empty on fire
class InfiniteShotgunAmmoPatch {
    static void Prefix(EnemyAI? ___heldByEnemy, ref int ___shellsLoaded, ref bool ___isReloading) {
        if (___heldByEnemy is not null || ___isReloading) return;
        if (___shellsLoaded <= 0) {
            ___shellsLoaded = 1;
        }
    }
}
