using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(StunGrenadeItem), nameof(StunGrenadeItem.SetExplodeOnThrowClientRpc))]
internal class EasterEggPatch
{
    private static void Postfix(StunGrenadeItem __instance)
    {
        if (!__instance.playerHeldBy.IsSelf()) return;
        var item = __instance.Reflect();
        var explodeOnThrow = item.GetInternalField<bool>("explodeOnThrow");
        if (explodeOnThrow) Helper.DisplayFlatHudMessage("This Easter Egg will Explode on drop!");
    }
}