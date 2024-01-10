#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.AttackPlayersServerRpc))]
class JebPatch {
    static void Prefix(ref bool ___attacking, ref bool ___inGrabbingObjectsAnimation, ref bool __state) {
        __state = ___inGrabbingObjectsAnimation;
        ___attacking = false;
        ___inGrabbingObjectsAnimation = false;
    }

    static void Postfix(ref bool ___inGrabbingObjectsAnimation, ref bool __state) {
        ___inGrabbingObjectsAnimation = __state;
    }
}
