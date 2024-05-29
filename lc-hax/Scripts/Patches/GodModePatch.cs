#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath))]
internal class GodModePatch
{
    private static bool Prefix(PlayerControllerB __instance, ref bool __result)
    {
        if (!Setting.EnableGodMode || !__instance.IsSelf()) return true;

        __result = false;
        __instance.inAnimationWithEnemy = null;
        __instance.inSpecialInteractAnimation = false;

        return false;
    }
}