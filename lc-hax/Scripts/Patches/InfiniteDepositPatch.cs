using HarmonyLib;

[HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.PlaceItemOnCounter))]
sealed class InfiniteDepositPatch {
    [HarmonyPostfix]
    [HarmonyInfix(150)]
    static void IncreaseLimit(ref int __result) => __result = int.MaxValue;
}
