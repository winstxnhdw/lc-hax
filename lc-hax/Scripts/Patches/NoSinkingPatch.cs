#pragma warning disable IDE1006
using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForSinkingInQuicksand")]
internal class NoSinking
{
    public static bool Prefix(PlayerControllerB __instance, ref bool __result)
    {
        if (!Setting.EnableNoSinking) return true;
        if (__instance.IsSelf())
        {
            __result = false;
            return false;
        }

        return true;
    }
}