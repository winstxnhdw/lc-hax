#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ItemActivate))]
class InfiniteShotgunAmmoPatch {
    static void Prefix(EnemyAI? ___heldByEnemy, ref int ___shellsLoaded) {
        if (___heldByEnemy is not null) return;
        ___shellsLoaded = 3;
    }
}
