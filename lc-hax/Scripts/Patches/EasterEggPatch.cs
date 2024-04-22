#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(StunGrenadeItem), nameof(StunGrenadeItem.SetExplodeOnThrowClientRpc))]
class EasterEggPatch {
    static void Postfix(StunGrenadeItem __instance) {
        if (!__instance.playerHeldBy.IsSelf()) return;
        Reflector<StunGrenadeItem> item = __instance.Reflect();
        bool explodeOnThrow = item.GetInternalField<bool>("explodeOnThrow");
        if (explodeOnThrow) {
            Helper.SendNotification("Explosive egg", "This Easter Egg will Explode on drop!", true);
        }
    }
}
